using M3U8.Library.Models;
using M3U8.Library.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3U8.Library
{
    public class M3U8Downloader
    {
        private HttpClient _httpClient = new HttpClient();
        private PlaylistParser _playlistParser = new PlaylistParser();
        private HashSet<string> downloadedBlocks = new HashSet<string>();
        private readonly Uri _downloadPath;

        public M3U8Downloader()
        {
            _downloadPath = new Uri(Directory.GetCurrentDirectory());
        }

        public M3U8Downloader(Uri downloadPath)
        {
            _downloadPath = downloadPath;
        }

        public Playlist Playlist { get; set; } = new Playlist();

        public Format SelectedFormat { get; set; }

        public void SetBaseURL(Uri url) => _httpClient.BaseAddress = url;

        public void SetHttpHeader(Dictionary<string, string> header)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            foreach (var pair in header)
            {
                _httpClient.DefaultRequestHeaders.Add(pair.Key, pair.Value);
            }
        }

        public async Task LoadPlaylistAsync(string url, string playlistName)
        {
            SetBaseURL(new Uri(url));

            using (var response = await _httpClient.GetAsync(playlistName))
            {
                if (response.IsSuccessStatusCode)
                {
                    string raw = await response.Content.ReadAsStringAsync();
                    Playlist = _playlistParser.GetPlaylist(raw, playlistName);
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task DownloadAsync(OnBlockDownloaded logger, CancellationToken cancellationToken)
        {
            var threads = new List<Task>();

            if (SelectedFormat == null)
            {
                throw new Exception("Select a Format");
            }

            bool stop = false;
            while (!stop)
            {
                stop = cancellationToken.IsCancellationRequested;

                int blockDuration = 1;
                logger("Getting a new chunk");
                string raw = await GetChunkData(SelectedFormat.FileName);
                string[] data = raw.Split("\n");

                for (int i = 0; i < data.Length; ++i)
                {
                    if (data[i].StartsWith("#EXT-X-TARGETDURATION:"))
                    {
                        blockDuration = int.Parse(data[i].Split(":")[1]);
                    }
                    else if (data[i].StartsWith("#EXTINF"))
                    {
                        if (downloadedBlocks.Contains(data[i + 1]))
                        {
                            logger($"{ data[i + 1] } Already downloaded");
                            continue;
                        }

                        downloadedBlocks.Add(data[i + 1]);
                        logger($"Downloading { data[i + 1] } @ { Path.Combine(_downloadPath.AbsolutePath, $"{ downloadedBlocks.Count }.ts") }");
                        threads.Add(DownloadAndSaveBlockAsync(data[i + 1], downloadedBlocks.Count.ToString()));
                        ++i;
                    }
                    else if(data[i] == "#EXT-X-ENDLIST")
                    {
                        stop = true;
                    }
                }
                await Task.Delay(blockDuration * 1000);
            }

            Task.WaitAll(threads.ToArray());
        }

        private async Task<string> GetChunkData(string chunk)
        {
            using var response = await _httpClient.GetAsync(chunk);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        private async Task DownloadAndSaveBlockAsync(string name, string filename)
        {
            var block = await DownloadBlockAsync(name);
            await File.WriteAllBytesAsync(
                Path.Combine(_downloadPath.AbsolutePath, $"{filename}.ts"), 
                block.Data
            );
        }

        private async Task<Block> DownloadBlockAsync(string name)
        {
            using var response = await _httpClient.GetAsync(name);
            if (response.IsSuccessStatusCode)
            {
                var block = new Block() { Name = name };
                block.Data = await response.Content.ReadAsByteArrayAsync();
                return block;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public delegate void OnBlockDownloaded(string msg);
    }
}
