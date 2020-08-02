using M3U8.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3U8.Library.Parsers
{
    internal class PlaylistParser
    {
        private readonly FormatParser _formatParser = new FormatParser();

        public Playlist GetPlaylist(string raw, string name)
        {
            var playlist = new Playlist() { FileName = name };

            string[] lines = raw.Split("\n");
            for (int i = 0; i < lines.Length; ++i)
            {
                switch (GetLineTag(lines[i]))
                {
                    case "#EXT-X-STREAM-INF":
                        playlist.Formats.Add(_formatParser.GetFormat(lines[i], lines[i + 1]));
                        ++i;
                        break;
                }
            }

            return playlist;
        }

        private string GetLineTag(string line)
        {
            int num = line.IndexOf(":");
            return num != -1 ? line[..num] : line;
        }
    }
}
