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
using System.Threading;
using System.Timers;
#endregion

namespace SingBot.Plugins {
	public class LuaInterp : Plugin {

        NLua.Lua lua = null;

        public struct RThread
        {
            public Thread t;
            public DateTime from;
        }
        public List<RThread> threads = new List<RThread>();
		#region " Constructor/Destructor "
        public LuaInterp(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);

            lua = new NLua.Lua();

            
		}
		#endregion


		#region " Methods "
        public void RunThread(Network n, Irc.IrcEventArgs e, string cmd)
        {
            try
            {
                var res = lua.DoString(cmd, "lua");
                if (res != null && res[0] != null)
                    n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Lua: " + Convert.ToString(res[0]));
            }
            catch (Exception x)
            {
                n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Error, sorry...");
                Console.WriteLine("Lua exception: " + x);
                return;
            }
        }

        public void TimeoutThread(object t)
        {
            
        }
		#endregion


		#region " Event handles "
		void Bot_OnMessage(Network n, Irc.IrcEventArgs e) 
        {
            if (!IsChannelEnabled(e.Data.Channel)) return;
            string[] args = e.Data.Message.Split(' ');
            if (args.Length < 1)
            {
                return;
            }
            else
            {
                if(args[0] == "!lua")
                {
                    if(args.Length == 1)
                    {
                        n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Usage: !lua [expression]");
                        return;
                    }

                    string cmd = "";

                    for(int i = 1; i < args.Length; i++)
                    {
                        cmd += args[i];
                        if (i != args.Length)
                            cmd += " ";
                    }
                    Thread t = new Thread(() => RunThread(n, e, cmd));
                    RThread rt = new RThread();
                    rt.t = t;
                    rt.from = DateTime.Now;
                    threads.Add(rt);
                    t.Start();
                    System.Timers.Timer timer = new System.Timers.Timer(5000);
                    timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
                        {
                            if (t.IsAlive)
                            {
                                Console.WriteLine("Aborting thread...");
                                t.Abort();
                            }
                            timer.Stop();
                            threads.Remove(rt);
                        };
                    timer.Start();
                }
            }
		}

		#endregion
	}
}
