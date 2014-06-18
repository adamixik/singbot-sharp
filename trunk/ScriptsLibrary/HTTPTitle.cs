/*
Sample script for the SingBot IRC Bot [http://singbot.unix-net.ru]
Copyright (C) 2014 adamix

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
#endregion

namespace SingBot.Scripts {
	public class HTTPTitle : Script {

		#region " Constructor/Destructor "
        public HTTPTitle(Bot bot)
			: base(bot) {
                Bot.OnChannelMessage += Bot_OnChannelMessage;
		}


		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e)
        {
            WebClient x = new WebClient();
            
            string[] args = e.Data.Message.Split(' ');

            if (args.Length == 1 && args[0] == "!title")
            {
                network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Пример: !title [url]");
                return;
            }
            else if(args.Length == 2 && args[0] == "!title")
            {
                try
                {
                    string source = x.DownloadString(new Uri(args[1]));
                    string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                    network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Title: " + title);
                }
                catch (Exception) 
                {
                    network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Error when detecting title.");
                }

            }
        }
        #endregion
    }
}
