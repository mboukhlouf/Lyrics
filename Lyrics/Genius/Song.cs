using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Lyrics.Genius
{
    public class Song
    {
        public String Title { get; set; }

        public String Artist { get; set; }  

        public String Url { get; set; }

        public String ThumbnailUrl { get; set; }
    }
}