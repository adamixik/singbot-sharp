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
using System.IO;
using System.Diagnostics;
#endregion

namespace SingBot.Scripts {

    public class UptimeCommand : SingBot.Command
    {
        public UptimeCommand()
            : base("uptime", "{pfx}uptime", Permissions.AccessLevel.ACCESS_OPERATOR, CommandType.COMMAND_TYPE_ALL, 0)
        {
            
        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            if(!Bot.GetSingleton().Scripts[System.Reflection.Assembly.GetExecutingAssembly().GetName().Name].IsChannelEnabled(e.Data.Channel)) return;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "/usr/bin/uptime";
            startInfo.Arguments = "";
            string output = "";
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                    StreamReader stream = exeProcess.StandardOutput;
                    output = stream.ReadToEnd();
                }
            }
            catch
            {
            }
            Console.WriteLine("Uptime: " + output);
            n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Server uptime: " + output.Trim());
        }
    }

    public class UnameCommand : SingBot.Command
    {
        public UnameCommand()
            : base("uname", "{pfx}uname", Permissions.AccessLevel.ACCESS_OPERATOR, CommandType.COMMAND_TYPE_ALL, 0)
        {

        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            if (!Bot.GetSingleton().Scripts[System.Reflection.Assembly.GetExecutingAssembly().GetName().Name].IsChannelEnabled(e.Data.Channel)) return;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "/bin/uname";
            startInfo.Arguments = "-a";
            string output = "";
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                    StreamReader stream = exeProcess.StandardOutput;
                    output = stream.ReadToEnd();
                }
            }
            catch
            {
            }
            Console.WriteLine("Uname: " + output); 
            n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Server: " + output.Trim());
        }
    }

	public class LinuxCommands : Script {

		#region " Constructor/Destructor "
        public LinuxCommands(Bot bot)
			: base(bot) {
                new UptimeCommand();
                new UnameCommand();
		}
		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        #endregion
    }
}
