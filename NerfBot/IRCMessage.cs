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
            int cBreakTrailerIndexIfExists = 2;
            int initialCount = cBreak.Length;
            if(initialCount == 1)
            {
                
            }
            List<string> commandAndParameters; //variable to hold the command and any parameters (excluding the trailer)
            if (string.IsNullOrEmpty(cBreak[0])) //if we have a blank first position that means we had a leading ':' and thus we have a prefix
            {
                string[] temp = cBreak[1].TrimEnd().Split((char)32); //split the first bit by space (this should actually be the prefix followed by everything but the trailer)
                Prefix = temp[0]; //Slice off the prefix
                commandAndParameters = temp.Skip(1).ToList(); //throw the rest of it into the gooeymiddle
                cBreakTrailerIndexIfExists++; //we had a prefix so the trailer would be one later
            }
            else //we don't have a prefix
            {
                commandAndParameters = cBreak[0].TrimEnd().Split((char)32).ToList(); //so the first position holds the command and parameters
            }
            ParseMiddle(commandAndParameters);
            if (initialCount == cBreakTrailerIndexIfExists)
            {
                Parameters.Add(cBreak[cBreakTrailerIndexIfExists - 1]);
            }
        }

        private void ParseMiddle(List<string> commandAndParameters)
        {
            Command = commandAndParameters.FirstOrDefault();
            Parameters = commandAndParameters.Skip(1).ToList();
        }
    }
}
