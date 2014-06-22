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
#endregion

namespace SingBot.Scripts {
    public class ColorTestCommand : SingBot.Command
    {
        public ColorTestCommand() : base("colortest", "{pfx}colortest [color]", Permissions.AccessLevel.ACCESS_NULL, CommandType.COMMAND_TYPE_CHANNEL, 1)
        {
            
        }
        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            if (!Bot.GetSingleton().Scripts[System.Reflection.Assembly.GetExecutingAssembly().GetName().Name].IsChannelEnabled(e.Data.Channel)) return;
            base.OnCommand(n, e, type, args);
            string msg = "colortest!";
            switch(args[0])
            {
                case "red":
                    {
                        msg = Script.FormatColor(msg, IrcColor.LightRed);
                        break; 
                    }
                case "blue":
                    {
                        msg = Script.FormatColor(msg, IrcColor.Blue);
                        break;
                    }
                case "yellow":
                    {
                        msg = Script.FormatColor(msg, IrcColor.Yellow);
                        break;
                    }
                case "bold":
                    {
                        msg = Script.FormatBold(msg);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            n.SendMessage(Irc.SendType.Message, e.Data.Channel, msg);
        }
    }

    public class RehashScripts : SingBot.Command
    {
        public RehashScripts() : base("rehashscripts", "{cmd}", Permissions.AccessLevel.ACCESS_NULL, CommandType.COMMAND_TYPE_CHANNEL, 0)
        {

        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            if (!Bot.GetSingleton().Scripts[System.Reflection.Assembly.GetExecutingAssembly().GetName().Name].IsChannelEnabled(e.Data.Channel)) return;
            base.OnCommand(n, e, type, args);
            Bot.GetSingleton().LoadScripts();
        } 
    }

	public class SimpleCommands : Script {

		#region " Constructor/Destructor "
        public SimpleCommands(Bot bot)
			: base(bot) {
                new ColorTestCommand();
                //new RehashScripts();
		}
		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        #endregion
    }
}
