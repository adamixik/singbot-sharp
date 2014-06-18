/*
Plugin for the SingBot IRC Bot [http://singbot.unix-net.ru]
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
using System.Xml;
using SingBot.Util;
#endregion

namespace SingBot.Plugins {
	public class Example : Plugin {

		#region " Constructor/Destructor "
		public Example(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);
		}
		#endregion


		#region " Methods "

		#endregion


		#region " Event handles "
		void Bot_OnMessage(Network n, Irc.IrcEventArgs e) {
            string[] args = e.Data.Message.Split(' ');
            if (args.Length < 1)
            {
                return;
            }
            else
            {
                if (args[0] == "!slab")
                {
                    if(args.Length != 2)
                    {
                        n.SendMessage(
                                       SingBot.Irc.SendType.Notice,
                                       e.Data.Nick, "!slab news - новости SecurityLab.ru.");
                        n.SendMessage(
                                       SingBot.Irc.SendType.Notice,
                                       e.Data.Nick, "!slab vulns - новости SecurityLab.ru о уязвимостях.");
                        n.SendMessage(
                                       SingBot.Irc.SendType.Notice,
                                       e.Data.Nick, "!slab viruses - новости SecurityLab.ru о вирусах.");

                        Console.WriteLine("Sending help...");
                        return;
                    }
                    if(args[1] == "news")
                    {
                        RssManager manager = new RssManager("http://www.securitylab.ru/_Services/Export/RSS/news/");
                        var feed = manager.GetFeed();
                        if (feed.Count < 5)
                            return;

                        for (int i = feed.Count-1; i > (feed.Count - 6); i-- )
                        {
                            var item = feed[i];
                            n.SendMessage(
                                SingBot.Irc.SendType.Notice,
                                e.Data.Nick,
                                "SLab News: " + item.Title + " ( " + item.Link + " )");
                        }

                    }

                    if (args[1] == "vulns")
                    {
                        RssManager manager = new RssManager("http://www.securitylab.ru/_Services/Export/RSS/vulnerabilities/");
                        var feed = manager.GetFeed();
                        if (feed.Count < 5)
                            return;

                        for (int i = feed.Count - 1; i > (feed.Count - 6); i--)
                        {
                            var item = feed[i];
                            n.SendMessage(
                                SingBot.Irc.SendType.Notice,
                                e.Data.Nick,
                                "SLab Vulns: " + item.Title + " ( " + item.Link + " )");
                        }

                    }

                    if (args[1] == "viruses")
                    {
                        RssManager manager = new RssManager("http://www.securitylab.ru/_Services/Export/RSS/virus/");
                        var feed = manager.GetFeed();
                        if (feed.Count < 5)
                        {
                            Console.WriteLine("ERROR!");
                            return;
                        }


                        for (int i = feed.Count - 1; i > (feed.Count - 6); i--)
                        {
                            var item = feed[i];
                            n.SendMessage(
                                SingBot.Irc.SendType.Notice,
                                e.Data.Nick,
                                "SLab Viruses: " + item.Title + " ( " + item.Link + " )");
                        }

                    }

                }

            }
		}

		#endregion
	}
}
