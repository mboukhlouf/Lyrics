using System.Net.Http;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Exception = System.Exception;

namespace Lyrics
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private class MusicBroadCastReceiver : BroadcastReceiver
        {
            public MainActivity MainActivity { get; set; }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                string cmd = intent.GetStringExtra("command");
                Log.Debug("mIntentReceiver.onReceive ", action + " / " + cmd);
                string artist = intent.GetStringExtra("artist");
                string album = intent.GetStringExtra("album");
                string track = intent.GetStringExtra("track");
                Log.Debug("Music", artist + ":" + album + ":" + track);
                MainActivity.UpdateLyrics(artist, track);
            }
        }

        private ImageView thumbnailImageView;
        private TextView songTextView;
        private TextView artistTextView;
        private TextView lyricsTextView;

        private ILyricsClient lyricsClient = new GeniusLyricsClient();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            thumbnailImageView = FindViewById<ImageView>(Resource.Id.thumbnailImageView);
            songTextView = FindViewById<TextView>(Resource.Id.songTextView);
            artistTextView = FindViewById<TextView>(Resource.Id.artistTextView);
            lyricsTextView = FindViewById<TextView>(Resource.Id.lyricsTextView);

            lyricsTextView.MovementMethod = new ScrollingMovementMethod();
            IntentFilter iF = new IntentFilter();

            // Android music player
            iF.AddAction("com.android.music.metachanged");
            // Spotify
            iF.AddAction("com.spotify.music.metadatachanged");

            RegisterReceiver(new MusicBroadCastReceiver {MainActivity = this}, iF);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void UpdateLyrics(string artist, string song)
        {
            string message;
            try
            {
                var search = artist + " " + song;
                var lyricsInfo = lyricsClient.SearchLyricsAsync(artist, song);
                songTextView.SetText(lyricsInfo.Song, TextView.BufferType.Normal);
                artistTextView.SetText(lyricsInfo.Artist, TextView.BufferType.Normal);
                lyricsTextView.SetText(lyricsInfo.Lyrics, TextView.BufferType.Normal);

                if(lyricsInfo.ThumbnailUrl != null)
                {
                    Thread thread = new Thread(() =>
                    {
                        HttpClient client = new HttpClient();
                        var stream = client.GetStreamAsync(lyricsInfo.ThumbnailUrl).GetAwaiter().GetResult();
                        Drawable d = Drawable.CreateFromStream(stream, "src");
                        MainThread.BeginInvokeOnMainThread(() => {
                            thumbnailImageView.SetImageDrawable(d);
                        });
                        stream.Dispose();
                        client.Dispose();
                    });
                    thread.Start();
                }

                message = $"{lyricsInfo.Artist} {lyricsInfo.Song}";
              //  ShowNotification(message);
            }
            catch (Exception e)
            {
                message = $"Exception: {e.Message}";
            }

            try
            {
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
            catch(Exception)
            {

            }
        }

        public void ShowNotification(string message)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentTitle("Lyrics") // title for notification
                .SetContentText(message) // message for notification
                .SetAutoCancel(true) // clear notification after click
                .SetPriority(NotificationCompat.PriorityLow);

            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(0, builder.Build());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        
    }
}

