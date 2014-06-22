/*
Hashes script for the SingBot IRC Bot [http://singbot.unix-net.ru]
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
using System.Security.Cryptography;
#endregion

namespace SingBot.Scripts {
	public class Hashes : Script {

		#region " Constructor/Destructor "
        public Hashes(Bot bot)
			: base(bot)
		{
			Bot.OnChannelMessage += new IrcEventHandler (Bot_OnMessage);
		}
		#endregion

		#region " Methods "
		
		public string ComputeHash(string input, HashAlgorithm algorithm)
		{
		   Byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
		
		   Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
		
		   return BitConverter.ToString(hashedBytes).Replace("-", "");
		}
		
		#endregion

		#region " Event handles "
		void Bot_OnMessage (Network n, Irc.IrcEventArgs e)
		{
            if (!IsChannelEnabled(e.Data.Channel)) return;
			string[] args = e.Data.Message.Split (' ');
            
            if (args.Length > 1) {

                string tohash = "";
                for (int i = 1; i < args.Length; i++ )
                {
                    tohash += args[i];
                    if (args.Length != i)
                        tohash += " ";
                }

                    if (args[0] == "!md5")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "MD5: " + ComputeHash(tohash, new MD5CryptoServiceProvider())
                        );
                    }
                    else if (args[0] == "!sha256")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "SHA256: " + ComputeHash(tohash, SHA256Managed.Create())
                        );
                    }
                    else if (args[0] == "!sha1")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "SHA1: " + ComputeHash(tohash, SHA1Managed.Create())
                        );
                    }
                    else if (args[0] == "!sha384")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "SHA384: " + ComputeHash(tohash, SHA384Managed.Create())
                        );
                    }
                    else if (args[0] == "!sha512")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "SHA512: " + ComputeHash(tohash, SHA512Managed.Create())
                        );
                    }
                    else if (args[0] == "!ripemd160")
                    {
                        n.SendMessage(
                            SingBot.Irc.SendType.Message,
                            e.Data.Channel,
                            "RIPEMD160: " + ComputeHash(tohash, RIPEMD160Managed.Create())
                        );
                    }
			}
            else if (args.Length == 1 && args[0] == "!hash")
            {
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "Hashing commands:");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!md5 [string]       - calculates MD5 hash.");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!sha1 [string]      - calculates SHA1 hash.");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!sha256 [string]    - calculates SHA256 hash.");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!sha384 [string]    - calculates SHA384 hash.");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!sha512 [string]    - calculates SHA512 hash.");
                n.SendMessage(SingBot.Irc.SendType.Notice, e.Data.Nick, "!ripemd160 [string] - calculates RIPEMD160 hash.");
            }
            else
            {
                return;
            }
		}
		#endregion

	}
}
