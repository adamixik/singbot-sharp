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
	public class ChannelLinking : Plugin {

        public class Link
        {
            public string NetworkMain;
            public string ChannelMain;
            public string NetworkSlave;
            public string ChannelSlave;
            public bool TranslateMainToSlave;
            public bool TranslateSlaveToMain;
        }

        public List<Link> links = new List<Link>();

		#region " Constructor/Destructor "
        public ChannelLinking(Bot bot)
			: base(bot) {
                Bot.OnJoin += Bot_OnJoin;
                Bot.OnPart += Bot_OnPart;
                Bot.OnQuit += Bot_OnQuit;

			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			//Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);

            var reader = new INIReader(AppDomain.CurrentDomain.BaseDirectory + "/ChannelLinking.ini");
            foreach(string section in reader.GetSections())
            {
                Link link = new Link();
                link.NetworkMain = reader.GetValue("NetworkMain", section);
                link.ChannelMain = reader.GetValue("ChannelMain", section);
                link.NetworkSlave = reader.GetValue("NetworkSlave", section);
                link.ChannelSlave = reader.GetValue("ChannelSlave", section);
                link.TranslateMainToSlave = Convert.ToBoolean(reader.GetValue("TranslateMainToSlave", section));
                link.TranslateSlaveToMain = Convert.ToBoolean(reader.GetValue("TranslateSlaveToMain", section));
                links.Add(link);
                Console.WriteLine("Loaded link: " + link.ChannelMain + "@" + link.NetworkMain + " to: " + link.ChannelSlave + "@" + link.NetworkSlave);
            }
		}





		#endregion


		#region " Methods "

		#endregion


		#region " Event handles "
		void Bot_OnMessage(Network n, Irc.IrcEventArgs e) 
        {
            foreach(Link l in links)
            {
                try
                {
                    if (l.NetworkMain == n.Name && l.ChannelMain == e.Data.Channel)
                    {
                        if (!l.TranslateMainToSlave) return;
                        var slave = Bot.GetNetworkByName(l.NetworkSlave);
                        slave.SendMessage(Irc.SendType.Message, l.ChannelSlave, "<" + e.Data.Nick + "@" + l.NetworkMain + ">: " + e.Data.Message);
                    }
                    else if (l.NetworkSlave == n.Name && l.ChannelSlave == e.Data.Channel)
                    {
                        if (!l.TranslateSlaveToMain) return;
                        var main = Bot.GetNetworkByName(l.NetworkMain);
                        main.SendMessage(Irc.SendType.Message, l.ChannelMain, "<" + e.Data.Nick + "@" + l.NetworkSlave + ">: " + e.Data.Message);
                    }
                }
                catch (Exception) { }
            }
		}
        void Bot_OnJoin(Network n, Irc.JoinEventArgs e)
        {
            foreach (Link l in links)
            {
                try
                {
                    if (l.NetworkMain == n.Name && l.ChannelMain == e.Data.Channel)
                    {
                        if (!l.TranslateMainToSlave) return;
                        var slave = Bot.GetNetworkByName(l.NetworkSlave);
                        slave.SendMessage(Irc.SendType.Message, l.ChannelSlave, "<" + e.Data.Nick + "@" + l.NetworkMain + "> joins " + l.ChannelMain);
                    }
                    else if (l.NetworkSlave == n.Name && l.ChannelSlave == e.Data.Channel)
                    {
                        if (!l.TranslateSlaveToMain) return;
                        var main = Bot.GetNetworkByName(l.NetworkMain);
                        main.SendMessage(Irc.SendType.Message, l.ChannelMain, "<" + e.Data.Nick + "@" + l.NetworkSlave + "> joins " + l.ChannelSlave);
                    }
                }
                catch (Exception) { }
            }
        }

        void Bot_OnPart(Network n, Irc.PartEventArgs e)
        {
            foreach (Link l in links)
            {
                try
                {
                    if (l.NetworkMain == n.Name && l.ChannelMain == e.Data.Channel)
                    {
                        if (!l.TranslateMainToSlave) return;
                        var slave = Bot.GetNetworkByName(l.NetworkSlave);
                        slave.SendMessage(Irc.SendType.Message, l.ChannelSlave, "<" + e.Data.Nick + "@" + l.NetworkMain + "> parts " + l.ChannelMain);
                    }
                    else if (l.NetworkSlave == n.Name && l.ChannelSlave == e.Data.Channel)
                    {
                        if (!l.TranslateSlaveToMain) return;
                        var main = Bot.GetNetworkByName(l.NetworkMain);
                        main.SendMessage(Irc.SendType.Message, l.ChannelMain, "<" + e.Data.Nick + "@" + l.NetworkSlave + "> parts " + l.ChannelSlave);
                    }
                }
                catch (Exception) { }
            }
        }

        void Bot_OnQuit(Network n, Irc.QuitEventArgs e)
        {
            foreach (Link l in links)
            {
                try
                {
                    if (l.NetworkMain == n.Name && l.ChannelMain == e.Data.Channel)
                    {
                        if (!l.TranslateMainToSlave) return;
                        var slave = Bot.GetNetworkByName(l.NetworkSlave);
                        slave.SendMessage(Irc.SendType.Message, l.ChannelSlave, "<" + e.Who + "@" + l.NetworkMain + "> quits " + l.ChannelMain + "( " + e.QuitMessage + " )");
                    }
                    else if (l.NetworkSlave == n.Name && l.ChannelSlave == e.Data.Channel)
                    {
                        if (!l.TranslateSlaveToMain) return;
                        var main = Bot.GetNetworkByName(l.NetworkMain);
                        main.SendMessage(Irc.SendType.Message, l.ChannelMain, "<" + e.Who + "@" + l.NetworkSlave + "> quits " + l.ChannelSlave + "( " + e.QuitMessage + " )");
                    }
                }
                catch (Exception) { }
            }
        }

		#endregion
	}
}
