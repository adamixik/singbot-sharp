/*
Command interface for the SingBot IRC Bot [http://singbot.unix-net.ru]
Copyright (C) 2012 adamix

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

using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;

namespace SingBot
{
	public enum CommandType
	{
		COMMAND_TYPE_CHANNEL,
		COMMAND_TYPE_QUERY,
		COMMAND_TYPE_ALL
	};
	public class Commands
	{
		public struct _Command
		{
			public readonly string cmd;
			public readonly string usage;
			public readonly Permissions.AccessLevel access;
			public readonly Command iface;
			public readonly int args;
			public readonly CommandType type;
			public _Command(string c, string u, Permissions.AccessLevel a, int args, CommandType type, Command i)
			{
				cmd = c;
				usage = u;
				access = a;
				iface = i;
				this.args = args;
				this.type = type;
			}
		};
        
		public static List<_Command> cmds = new List<_Command>();
		public Commands()
		{
			
		}
		public static _Command Add (string cmd, string usage, Permissions.AccessLevel access, int args, CommandType type, Command iface)
		{
			_Command command = new _Command (cmd, usage, access, args, type, iface);
			cmds.Add (command);
			return command;
		}
	}
	public class Command
	{
		private Commands._Command _command;
        protected List<string> Aliases = new List<string>();
		public Command (string cmd, string usage, Permissions.AccessLevel access, CommandType type, int args)
		{
			_command = Commands.Add (cmd, usage.Replace("{pfx}", Bot.GetSingleton().GetCommandPrefix()), access, args, type, this);
			
			Bot.GetSingleton ().OnChannelMessage += new IrcEventHandler (_OnChannelMessage);
			Bot.GetSingleton ().OnQueryMessage += new IrcEventHandler (_OnQueryMessage);
		}
		private void _OnChannelMessage (Network n, Irc.IrcEventArgs e)
		{
			string[] args = e.Data.Message.Split (' ');
			Permissions.AccessLevel level = Bot.GetSingleton ().GetPermissions ().GetAccess (Bot.GetSingleton ().GetPermissions ()
				.GetLogin (e, n)
			);
			if (args [0] == Bot.GetSingleton ().GetCommandPrefix () + _command.cmd || Aliases.Contains(args[0])) {
                List<string> a = new List<string>();
                for (int i = 1; i < args.Length;i++ )
                {
                    a.Add(args[i]);
                }
                    if (level < _command.access)
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            e.Data.Nick + ": You dont have access to use this command."
                        );
                        return;
                    }
                if (args.Length < _command.args + 1 && _command.args != -1)
                {
					n.SendMessage (
						SingBot.Irc.SendType.Message,
						e.Data.Channel,
						e.Data.Nick + ", Usage: " + _command.usage.Replace("{cmd}", args[0])
					);
					return;
				}
				this.OnCommand (n, e, _command.type, a);
			}
		}
		private void _OnQueryMessage (Network n, Irc.IrcEventArgs e)
		{
			string[] args = e.Data.Message.Split (' ');
			Permissions.AccessLevel level = Bot.GetSingleton ().GetPermissions ().GetAccess (Bot.GetSingleton ().GetPermissions ()
				.GetLogin (e, n)
			);
            if (args[0] == Bot.GetSingleton().GetCommandPrefix() + _command.cmd || Aliases.Contains(args[0]))
            {
                List<string> a = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    a.Add(args[i]);
                }
				if (level < _command.access) {
					n.SendMessage (
						SingBot.Irc.SendType.Message,
						e.Data.Nick,
						"You dont have access to use this command."
					);
					return;
				}
				if (args.Length != _command.args + 1 && _command.args != -1) {
					n.SendMessage (
						SingBot.Irc.SendType.Message,
						e.Data.Nick,
                        "Usage: " + _command.usage.Replace("{cmd}", args[0])
					);
					return;
				}
				this.OnCommand (n, e, _command.type, a);
			}
		}
		public virtual void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
		{
			
		}
	}
	
	public class PlusCommand : SingBot.Command
	{
		public PlusCommand() : base("plus", "{pfx}plus [num 1] [num 2]", SingBot.Permissions.AccessLevel.ACCESS_USER, SingBot.CommandType.COMMAND_TYPE_ALL, 2)
		{
			
		}
		public override void OnCommand (Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
		{
			int num1 = Int32.Parse(args [0]);
			int num2 = Int32.Parse(args [1]);
			
			n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "Answer: " + (num1 + num2).ToString());
			
		}
	}
	
}