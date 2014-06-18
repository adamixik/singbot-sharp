/*
Plugin for the SingBot IRC Bot [http://singbot.unix-net.ru]
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

#region Using directives
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Mono.CSharp;
#endregion

namespace SingBot.Plugins {
	public class CSharpInterp : Plugin {

		private ReportPrinter printer;
		private CompilerSettings settings;
		private CompilerContext ctx;
		private Evaluator evaluator;
		private TextWriter agent_stderr;
		private MemoryStream mem_stream;
		
		#region " Constructor/Destructor "
		public CSharpInterp (Bot bot)
			: base(bot)
		{
			Bot.OnChannelMessage += new IrcEventHandler (Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler (Bot_OnMessage);
			
			settings = new CompilerSettings ();
			mem_stream = new MemoryStream ();
			agent_stderr = new StreamWriter (mem_stream);
			printer = new StreamReportPrinter (agent_stderr);
			
			ctx = new CompilerContext (settings, printer);
			
			evaluator = new Evaluator (ctx);
			evaluator.Run ("using System; using System.Linq; using System.Collections.Generic; using System.Collections;");
		}
		#endregion

		private string Evaluate (string input, out bool error)
		{
						
			bool result_set;
			object result;
			try {
				string inp = evaluator.Evaluate (input, out result, out result_set);

				if (result_set) {
					error = false;
					return (string)result;
				}
			} catch (Exception e) {
				error = true;
				return e.ToString ();
			}

			error = false;
			return null;
		}

		#region " Methods "

		
		
		#endregion


		#region " Event handles "
		void Bot_OnMessage (Network n, Irc.IrcEventArgs e)
		{
			Permissions.AccessLevel level = Bot.GetPermissions ().GetAccess (Bot.GetPermissions ().GetLogin (e, n));
			if (level < Permissions.AccessLevel.ACCESS_FULL)
				return;
			if (e.Data.Message.StartsWith ("!csharp ")) {
				string code = e.Data.Message.Substring (8);
				bool error;
				long pos = mem_stream.Position;
				string outstr = Evaluate (code, out error);
				
				agent_stderr.Flush ();
				mem_stream.Position = pos;
				StreamReader reader = new StreamReader (mem_stream);
				string stderr = reader.ReadToEnd();
				
				if (error || !String.IsNullOrEmpty(stderr)) {
					Answer (n, e, "Error: " + stderr);
				}
				else
				{
					if(outstr != null)
						Answer (n, e, outstr);
				}
			}
		}

		#endregion
	}
}
