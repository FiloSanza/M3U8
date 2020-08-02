using M3U8.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace M3U8.Library.Parsers
{
    internal class FormatParser
    {
        private const string SPLIT_PATTERN = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

        public Format GetFormat(string header, string filename)
        {
            var format = new Format() { FileName = filename };
            header = header.Replace("#EXT-X-STREAM-INF:", string.Empty);

            foreach (var tag in Regex.Split(header, SPLIT_PATTERN))
            {
                string[] pair = tag.Split("=");
                string key = pair[0];
                string value = pair[1];
                
                if (key == "BANDWIDTH")
                {
                    format.Bandwidth = int.Parse(value);
                }
                else if (key == "RESOLUTION")
                {
                    string[] resolution = value.Split("x");
                    format.Width = int.Parse(resolution[0]);
                    format.Height = int.Parse(resolution[1]);
                }
                else if (key == "CODECS")
                {
                    format.Codecs = value;
                }
                else if (key == "AVERAGE-BANDWIDTH")
                {
                    format.AverageBandwidth = int.Parse(value);
                }
                else if (key == "FRAME-RATE")
                {
                    format.FrameRate = int.Parse(value);
                }
            }

            return format;
        }
    }
}
