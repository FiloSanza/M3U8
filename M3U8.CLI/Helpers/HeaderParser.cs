using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace M3U8.CLI.Helpers
{
    public class HeaderParser
    {
        public Dictionary<string, string> GetHeader(string fileName)
        {
            var output = new Dictionary<string, string>();
            
            foreach (var line in File.ReadAllText(fileName).Split("\n"))
            {
                string[] pair = line.Split("|");
                output.Add(
                    pair[0].Trim(), 
                    pair[1].Trim()
                );
            }
            
            return output;
        }
    }
}
