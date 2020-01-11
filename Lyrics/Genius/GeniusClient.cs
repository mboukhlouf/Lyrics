using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Lyrics.Genius
{
    public class GeniusClient : IDisposable
    {
        private HttpClientHandler httpClientHandler;
        private HttpClient httpClient;
        private bool disposed = false;

        public String Token { get; set; } = "s6dofbetBGkrsJ0OQH5IPEEyLla4XJ9-iHVTyMy01h1Jvl_1b_spO4cTQ09Me2T1";

        public String UserAgent { get; set; } = "Lyrics";

        public GeniusClient()
        {
            httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }

            httpClientHandler.Dispose();
            httpClient.Dispose();

            disposed = true;
        }

        ~GeniusClient()
        {
            Dispose(false);
        }

        public Song Search(String q)
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.genius.com/search?q={q}")
            };

            Song song = null;
            var response = Send(message);

            if (response.IsSuccessStatusCode)
            {
                song = new Song();
                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JObject bodyJson = JObject.Parse(body);
                JObject firstHit = (JObject)((JArray)bodyJson["response"]["hits"])[0]["result"];
                song.Title = (String)firstHit["title"];
                song.Url = (String)firstHit["url"];
                song.ThumbnailUrl = (String)firstHit["song_art_image_thumbnail_url"];
                song.Artist = (String) firstHit["primary_artist"]["name"];
            }

            message.Dispose();
            response.Dispose();
            return song;
        }

        public HttpResponseMessage Send(HttpRequestMessage message)
        {
            // Accept-Encoding
            if (!message.Headers.Contains("Accept-Encoding"))
                message.Headers.Add("Accept-Encoding", "gzip, deflate, br");

            // User-Agent
            if (!message.Headers.Contains("User-Agent"))
                message.Headers.Add("User-Agent", UserAgent);

            // Authorization
            if (!message.Headers.Contains("Authorization"))
                message.Headers.Add("Authorization", $"Bearer {Token}");

            return httpClient.SendAsync(message).GetAwaiter().GetResult();
        }
    }
}