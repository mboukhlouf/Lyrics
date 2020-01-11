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
    public class GeniusApiHelper
    {
        public static Uri BaseUrl => new Uri("https://api.genius.com");
    }
}