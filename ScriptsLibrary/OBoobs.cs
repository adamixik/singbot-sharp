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
#endregion

namespace SingBot.Scripts {
	public class OBoobs : Script {

        int maxId = 8400;

		#region " Constructor/Destructor "
        public OBoobs(Bot bot)
			: base(bot) {
                Bot.OnChannelMessage += Bot_OnChannelMessage;
		}
		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e)
        {
            Random rand = new Random();
            string[] args = e.Data.Message.Split(' ');
            if(args.Length == 1)
            {
                if(args[0] == "!tits" || args[0] == "!boobs" || args[0] == "!сиськи")
                {

                    while(true)
                    {
                        int id = rand.Next(1, maxId);
                        string done = id.ToString();
                        if (done.Length < 5)
                        {
                            for (int i = done.Length; i < 5; i++)
                            {
                                done = done.Insert(0, "0");
                            }
                        }

                        string url = "http://media.oboobs.ru/boobs/" + done + ".jpg";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Timeout = 3000;
                        request.Method = "GET";

                        try
                        {
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            if(response.StatusCode == HttpStatusCode.NotFound)
                            {
                                continue;
                            }
                        }
                        catch
                        {
                        }

                        network.SendMessage(Irc.SendType.Notice, e.Data.Nick, "СИСЬКИ: " + url);
                        break;
                    }

                }
            }
        }
        #endregion

    }
}
