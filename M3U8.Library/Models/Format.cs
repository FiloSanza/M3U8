using System;
using System.Collections.Generic;
using System.Text;

namespace M3U8.Library.Models
{
    public class Format
    {
        public Format(string fileName, string codecs, int bandwidth, int width, int height)
        {
            FileName = fileName;
            Codecs = codecs;
            Bandwidth = bandwidth;
            Width = width;
            Height = height;
        }

        public Format()
        {

        }

        public string FileName { get; set; }
        public string Codecs { get; set; }
        public int Bandwidth { get; set; }
        public int AverageBandwidth { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameRate { get; set; }
    }
}
