using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3U8.CLI
{
    public class Options
    {
        [Option('v', "verbose", Default = true, HelpText = "Prints all messages to standard output.", Required = false)]
        public bool Verbose { get; set; }

        [Option('u', "url", HelpText = "The base URL of the playlist, i.e. http://www.asd.com/", Required = true)]
        public string BaseUrl { get; set; }

        [Option('p', "playlist", Default = "playlist.m3u8", HelpText = "The name of the main playlist file.", Required = false)]
        public string PlaylistName { get; set; }

        [Option('f', "format", Default = 0, HelpText = "The format to download.", Required = false)]
        public int FormatSelected { get; set; }

        [Option("download-path", Default = "", HelpText = "Set the download directory.\nCWD by default.", Required = false)]
        public string DownloadDirectory { get; set; }

        [Option("format-file", HelpText = "Use a custom format file, useful when playlist file is not available.", Required = false)]
        public string FormatFile { get; set; }

        [Option("header-file", HelpText = "Path to a file with the Http headers needed.\nThe file must have one key-value pair in each line separated by a pipe '|'.", Required = false)]
        public string HeaderFile { get; set; }
    }
}
