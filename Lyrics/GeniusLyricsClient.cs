using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;
using Lyrics.Genius;
using Newtonsoft.Json.Linq;

namespace Lyrics
{
    class GeniusLyricsClient : ILyricsClient
    {
        public LyricsInfo SearchLyricsAsync(string artist, string song)
        {
            var geniusClient = new GeniusClient();
            Song songInfo = geniusClient.Search($"{artist} {song}");

            String uri = songInfo.Url;
            var client = new HttpClient();

            var response = client.GetAsync(uri).GetAwaiter().GetResult();

            LyricsInfo lyricsInfo = null;
            if (response.IsSuccessStatusCode)
            {
                lyricsInfo = new LyricsInfo();
                lyricsInfo.Artist = songInfo.Artist;
                lyricsInfo.Song = songInfo.Title;
                lyricsInfo.ThumbnailUrl = songInfo.ThumbnailUrl;

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(body);

                var lyricsDiv = doc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                lyricsInfo.Lyrics = lyricsDiv.InnerText.Trim();
            }

            response.Dispose();
            client.Dispose();
            return lyricsInfo;
        }
    }
}