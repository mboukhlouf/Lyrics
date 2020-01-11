using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Xamarin.Android.Net;

namespace Lyrics
{
    public class MouritsLyricsClient : ILyricsClient
    {
        public LyricsInfo SearchLyricsAsync(String artist, String song)
        {
            String uri = $"https://mourits.xyz:2096/?a={artist}&s={song}";
            var client = new HttpClient();

            var response = client.GetAsync(uri).GetAwaiter().GetResult();
            var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            JObject bodyJson = JObject.Parse(body);
            LyricsInfo info = null;
            if (bodyJson["success"] != null && (bool)bodyJson["success"])
            {
                info = new LyricsInfo
                {
                    Artist = (String)bodyJson["artist"],
                    Song = (String)bodyJson["song"],
                    Lyrics = (String)bodyJson["result"]["lyrics"]
                };
            }
            client.Dispose();
            return info;
        }
    }
}