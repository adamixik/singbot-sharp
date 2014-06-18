/*
Sample Welcome script for the SingBot IRC Bot [http://singbot.unix-net.ru]
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
using System.Threading;
#endregion

namespace SingBot.Scripts {
	public class SampleWelcome : Script {

		#region " Constructor/Destructor "
        public SampleWelcome(Bot bot)
			: base(bot) {
			Bot.OnJoin += new JoinEventHandler(Bot_OnJoin);
		}
		#endregion

		#region " Methods "
		#endregion

		#region " Event handles "
		void Bot_OnJoin(Network n, Irc.JoinEventArgs e) {
			string s = e.Data.Nick + ", Hello from CSharp!";
			Answer(n, e, s);
		}
		#endregion

	}
}
