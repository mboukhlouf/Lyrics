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

namespace Lyrics
{
    class LyricsInfo
    {
        public String Artist { get; set; }

        public String Song { get; set; }

        public String Lyrics { get; set; }
    }
}