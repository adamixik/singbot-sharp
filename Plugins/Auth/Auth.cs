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
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
#endregion

namespace SingBot.Plugins {
	public class Auth : Plugin {
		
		[Serializable]
		public struct SimpleUser
		{
			public string Login { get; set; }
			public string Password { get; set; }
			public SingBot.Permissions.AccessLevel Access { get; set; }
		};
		
		private List<SimpleUser> users;
		
		
		
		#region " Constructor/Destructor "
		public Auth (Bot bot)
			: base(bot)
		{
			if (!Directory.Exists ("Users/")) {
				Console.WriteLine ("Config directory 'Users/' not found!");
				return;
			}
			
			users = new List<SimpleUser> ();
		
			foreach (FileInfo file in new DirectoryInfo ("Users").GetFiles()) {
				SimpleUser user = new SimpleUser ();
				XmlSerializer serializer = new XmlSerializer (user.GetType ());
				TextReader reader = new StreamReader (file.FullName);
				user = (SimpleUser)serializer.Deserialize (reader);
				users.Add (user);
				Console.WriteLine("Loaded user: {0} with access level {1}...", user.Login, user.Access.ToString());
			}
			
			

			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);
		}
		#endregion


		#region " Methods "

		private bool IsUserRegistered (string login)
		{
			foreach (FileInfo file in new DirectoryInfo ("Users").GetFiles()) {
				if (file.Name == login + ".user")
					return true;
			}
			return false;
		}
		
		private SimpleUser GetUserByLogin (string login)
		{
			foreach (SimpleUser user in users) {
				if (user.Login == login)
					return user;
			}
			return new SimpleUser ();
		}
		
		public string ComputeHash(string input, HashAlgorithm algorithm)
		{
		   Byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
		
		   Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
		
		   return BitConverter.ToString(hashedBytes);
		}
		
		#endregion


		#region " Event handles "
		void Bot_OnMessage (Network n, Irc.IrcEventArgs e)
		{
			string[] message = e.Data.Message.Split (' ');
			if (message [0] == "!register" && message.Length == 3) {
				if (IsUserRegistered (message [1])) {
					n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "This login name is already registered!");
					return;
				} else {
					SimpleUser user = new SimpleUser ();
					user.Login = message [1];
					user.Password = ComputeHash (message [1] + message [2], SHA256Managed.Create ());
					user.Access = SingBot.Permissions.AccessLevel.ACCESS_USER;
					XmlSerializer ser = new XmlSerializer (user.GetType ());
					TextWriter writer = new StreamWriter ("Users/" + message [1] + ".user");
					ser.Serialize (writer, user);
					users.Add (user);
					n.SendMessage (
						SingBot.Irc.SendType.Message,
						e.Data.Nick,
						"You have been registered, use !login for authentication."
					);
					return;
				}
			}
			
			if (message [0] == "!login" && message.Length == 3) {
				if (!IsUserRegistered (message [1])) {
					n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "This login name isn't registered.");
					return;
				} else {
					SimpleUser user = GetUserByLogin (message [1]);
					if (user.Password == ComputeHash (message [1] + message [2], SHA256Managed.Create ())) {
						Bot.GetPermissions ().Login (message [1], e.Data.Nick, e.Data.Host, e.Data.Ident, n, user.Access);
						n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "You have successfully logged in!");
						return;
					} else {
						n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "Wrong password!");
						return;
					}
				}
			}
			if (message [0] == "!logoff") {
				string login = Bot.GetPermissions ().GetLogin (e, n);
				if (login != null) {
					Bot.GetPermissions ().Logoff (login);
					n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "Good bye.");
				} else {
					n.SendMessage (SingBot.Irc.SendType.Message, e.Data.Nick, "You're not logged in!");
				}
			}
		}

		#endregion
	}
}
