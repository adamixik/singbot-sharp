/*
Plugin for the SingBot IRC Bot [http://singbot.unix-net.ru]
Copyright (C) 2010 adamix

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
using Starksoft.Net.Proxy;
using System.Net;
using System.Threading;
using System.IO;
#endregion

namespace SingBot.Plugins {


    public class ProxyCheckCommand : SingBot.Command
    {
        int[] ports = { 80, 8080, 443, 1080, 8888, 3128, 3127, 9999, 7780 };
        List<string> words = new List<string>();
        Random rand = new Random();
        public ProxyCheckCommand()
            : base("proxycheck", "{pfx}proxycheck [address]", Permissions.AccessLevel.ACCESS_NULL, CommandType.COMMAND_TYPE_CHANNEL, 1)
        {

        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            if (!Bot.GetSingleton().Scripts[System.Reflection.Assembly.GetExecutingAssembly().GetName().Name].IsChannelEnabled(e.Data.Channel)) return;
            base.OnCommand(n, e, type, args);
            string addr = args[0];
            Thread t = new Thread(() => CheckThreadHTTP(n, e, addr));
            t.Start();
        }

        public void CheckThreadHTTP(Network n, Irc.IrcEventArgs e, string address)
        {
            for (int i = 0; i < ports.Length; i++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://google.com");
                WebProxy myproxy = new WebProxy(address, ports[i]);
                myproxy.BypassProxyOnLocal = false;
                request.Proxy = myproxy;
                request.Method = "GET";
                request.Timeout = 5000;
                request.ReadWriteTimeout = 5000;
                try
                {
                    bool ok = false;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    
                    // Checking if anonymous ( not important)
                    StreamReader sr = new StreamReader(stream);
                    while (sr.EndOfStream != true)
                    {
                        string checking = sr.ReadLine();
                        Console.WriteLine(checking);
                        if (checking.Contains("Google"))
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok)
                    {
                        n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Found proxy on: " + address + ":" + ports[i]);
                        Thread.CurrentThread.Abort();
                    }
                    else
                    {
                        Console.WriteLine("Not found: " + ports[i]);
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Not found: " + ports[i]);
                }
            }
            n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Not found proxy on " + address);
        }
    }

	public class ProxyChecker : Plugin {
        int[] ports = { 80, 8080, 443, 1080, 8888, 3128, 3127, 9999, 7780 };
		#region " Constructor/Destructor "
		public ProxyChecker(Bot bot)
			: base(bot) {

			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);

            new ProxyCheckCommand();
		}
		#endregion


		#region " Methods "

		#endregion


		#region " Event handles "
		void Bot_OnMessage(Network n, Irc.IrcEventArgs e) {

		}

		#endregion
	}
}
