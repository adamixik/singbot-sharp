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
	public class AutoRejoin : Script {

		#region " Constructor/Destructor "
        public AutoRejoin(Bot bot)
			: base(bot) {
            Bot.OnKick += Bot_OnKick;
		}

		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        void Bot_OnKick(Network network, Irc.KickEventArgs e)
        {
            if(e.Whom == network.Nickname)
            {
                network.RfcJoin(e.Channel);
            }
        }
        #endregion

    }
}
