/*
Log Plugin for the SingBot IRC Bot [http://singbot.unix-net.ru]
Copyright (C) 2010-2014 adamix

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
#endregion

namespace SingBot.Plugins {
	public class Log : Plugin {

		#region " Constructor/Destructor "
		public Log(Bot bot)
			: base(bot) {
			Bot.OnRawMessage += new IrcEventHandler(Bot_OnRawMessage);

			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);
		}
		#endregion

		#region " Event handles "
		void Bot_OnRawMessage(Network network, Irc.IrcEventArgs e) {


		}

		void Bot_OnMessage(Network n, Irc.IrcEventArgs e) {
            if (!IsChannelEnabled(e.Data.Channel)) return;
            DateTime d = DateTime.Now;
            try
            {
                string file = "Logs/" + n.Name + "/" + e.Data.Channel + "/";
                System.IO.Directory.CreateDirectory(file);
                file += d.Year + "." + d.Month + "." + d.Day + ".log";
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file, true);
                writer.WriteLine(d.Hour + ":" + d.Minute + ":" + d.Second + " <" + e.Data.Nick + ">: " + e.Data.Message);
                writer.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine("#" + exception.Message);
            }
		}
		#endregion
	}
}
