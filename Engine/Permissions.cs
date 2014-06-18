/*
SingBot: The C# IRC Bot
Copyright (C) 2010 The SingBot Project

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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;

namespace SingBot
{
	public class Permissions
	{
		private Dictionary<string, AccessUser> users = null;
		
		public enum AccessLevel
		{
			ACCESS_NULL,
			ACCESS_USER,
			ACCESS_OPERATOR,
			ACCESS_ADMIN,
			ACCESS_OWNER,
			ACCESS_FULL
		};
		
		public struct AccessUser
		{
			public string Nick;
			public string Host;
			public string Ident;
			public Network Network;
			public AccessLevel Access;
			
			public bool IsValid ()
			{
				return !string.IsNullOrEmpty (Nick); 
			}
		};
		
		public Permissions ()
		{
			users = new Dictionary<string, AccessUser> ();
		}
		
		public bool Login (string login, string nick, string host, string ident, Network network, AccessLevel access)
		{
			if (users.ContainsKey (login))
				return false;
			
			AccessUser user = new AccessUser ();
			user.Nick = nick;
			user.Host = host;
			user.Ident = ident;
			user.Network = network;
			user.Access = access;
			users.Add (login, user);
			
			return true;
		}
		
		public bool Logoff (string login)
		{
			if (!users.ContainsKey (login))
				return false;
			
			users.Remove (login);
			
			return true;
		}
		
		public AccessLevel GetAccess (string login)
		{
			if (login == null)
				return AccessLevel.ACCESS_NULL;
			
			if (!users.ContainsKey (login))
				return AccessLevel.ACCESS_NULL;
			
			return users [login].Access;
		}
		
		public Network GetNetwork (string login)
		{
			if (!users.ContainsKey (login))
				return null;
			
			return users [login].Network;
		}
		
		public string GetLogin (string nick, string host, string ident, Network network)
		{
			foreach (var pair in users) {
				if (pair.Value.Nick == nick && pair.Value.Host == host && pair.Value.Ident == ident && pair.Value.Network == network) {
					return pair.Key;
				}
			}
			return null;
		}
		
		public string GetLogin (Irc.IrcEventArgs args, Network network)
		{
			foreach (var pair in users) {
				if (pair.Value.Nick == args.Data.Nick && pair.Value.Host == args.Data.Host && pair.Value.Ident == args.Data.Ident && pair.Value.Network == network) {
					return pair.Key;
				}
			}
			return null;
		}
		
		public AccessUser GetAccessUser (string nick, string host, string ident, Network network)
		{
			foreach (var pair in users) {
				if (pair.Value.Nick == nick && pair.Value.Host == host && pair.Value.Ident == ident && pair.Value.Network == network) {
					return pair.Value;
				}
			}
			return new AccessUser();
		}
		
		public AccessUser GetAccessUser (Irc.IrcEventArgs args, Network network)
		{
			foreach (var pair in users) {
				if (pair.Value.Nick == args.Data.Nick && pair.Value.Host == args.Data.Host && pair.Value.Ident == args.Data.Ident && pair.Value.Network == network) {
					return pair.Value;
				}
			}
			return new AccessUser();
		}
	}
}