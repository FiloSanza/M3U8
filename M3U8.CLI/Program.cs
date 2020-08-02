using CommandLine;
using M3U8.CLI.Helpers;
using M3U8.Library;
using M3U8.Library.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3U8.CLI
{
    class Program
    {
        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(StopDownload);

            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(RunWithOptions);
        }

        private static async Task RunWithOptions(Options options)
        {
            var m3u8 = await ConfigureDownloader(options);

            await m3u8.DownloadAsync(
                (string msg) => 
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine(msg);
                    }
                },
                cancellationTokenSource.Token
            );
        }

        private static async Task<M3U8Downloader> ConfigureDownloader(Options options)
        {
            var m3u8 = new M3U8Downloader();

            if (string.IsNullOrEmpty(options.DownloadDirectory))
            {
                options.DownloadDirectory = Directory.GetCurrentDirectory();
            }

            m3u8 = new M3U8Downloader(new Uri(options.DownloadDirectory));

            if (!string.IsNullOrEmpty(options.HeaderFile))
            {
                var headerParser = new HeaderParser();
                m3u8.SetHttpHeader(headerParser.GetHeader(options.HeaderFile));
            }

            if (string.IsNullOrEmpty(options.FormatFile))
            {
                await m3u8.LoadPlaylistAsync(options.BaseUrl, options.PlaylistName);
                m3u8.SelectedFormat = m3u8.Playlist.Formats[options.FormatSelected];
            }
            else
            {
                m3u8.Playlist = new Playlist();
                m3u8.SetBaseURL(new Uri(options.BaseUrl));
                m3u8.Playlist.Formats.Add(new Format() { FileName = options.FormatFile });
                m3u8.SelectedFormat = m3u8.Playlist.Formats[0];
            }

            return m3u8;
        }

        private static void StopDownload(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Terminating the execution");
            cancellationTokenSource.Cancel();
            args.Cancel = true;
        }
    }
}
