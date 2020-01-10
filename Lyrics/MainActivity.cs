using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
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

        private TextView songTextView;
        private TextView artistTextView;
        private TextView lyricsTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


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
            try
            {
                var search = artist + " " + song;
                var lyricsInfo = LyricsClient.SearchLyricsAsync(artist, song);
                songTextView.SetText(lyricsInfo.Song, TextView.BufferType.Normal);
                artistTextView.SetText(lyricsInfo.Artist, TextView.BufferType.Normal);
                lyricsTextView.SetText(lyricsInfo.Lyrics, TextView.BufferType.Normal);

                Toast.MakeText(this, search, ToastLength.Short)
                    .Show();
            }
            catch (Exception e)
            {
                var message = $"Exception: {e.Message}";
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        
    }
}

