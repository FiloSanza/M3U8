using System;
using System.Collections.Generic;
using System.Text;

namespace M3U8.Library.Models
{
    public class Playlist
    {
        public string FileName { get; set; }
        public List<Format> Formats { get; set; } = new List<Format>();
    }
}
