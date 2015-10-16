using System;
using System.Collections.Generic;
using System.Linq;

namespace NerfBot
{
    public class IRCMessage
    {
        public string Prefix { get; set; }
        public string Command { get; set; }
        public List<string> Parameters { get; set; }

        public IRCMessage(String rawMessage)
        {
            string[] cBreak = rawMessage.Trim().Split(':');
            int posibleTrailerCount = 2;
            int initialCount = cBreak.Length;
            List<string> gooeyMidle; //not the prefix or trailer
            if (string.IsNullOrEmpty(cBreak[0]))
            {
                string[] temp = cBreak[1].TrimEnd().Split((char)32);
                Prefix = temp[0];
                gooeyMidle = temp.Skip(1).ToList();
                posibleTrailerCount++;
            }
            else
            {
                gooeyMidle = cBreak[0].TrimEnd().Split((char)32).ToList();
            }
            Command = gooeyMidle[0];
            gooeyMidle.RemoveAt(0);
            Parameters = gooeyMidle;
            if (initialCount == posibleTrailerCount)
            {
                Parameters.Add(cBreak[posibleTrailerCount - 1]);
            }
        }
    }
}
