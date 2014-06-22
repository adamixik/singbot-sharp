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
using System.Net;
using System.Text.RegularExpressions;
using System.Data;
#endregion

namespace SingBot.Scripts {
	public class Calculator : Script {

		#region " Constructor/Destructor "
        public Calculator(Bot bot)
			: base(bot) {
                Bot.OnChannelMessage += Bot_OnChannelMessage;
		}


		#endregion

		#region " Methods "
		#endregion

        #region " Events "
        void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e)
        {
            if (!IsChannelEnabled(e.Data.Channel)) return;
            string[] args = e.Data.Message.Split(' ');

            if (args.Length == 1 && args[0] == "!calc")
            {
                network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Использование: !calc [expression], Пример: !calc 1 + 5");
                return;
            }
            if(args.Length > 1 && args[0] == "!calc")
            {
                string calc = "";
                for (int i = 1; i < args.Length; i++)
                {
                    calc += args[i];
                    if (args.Length != i)
                        calc += " ";
                }
                try
                {
                    string value = new DataTable().Compute(calc, null).ToString();
                    network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Результат: " + value);
                }
                catch (Exception)
                {
                    network.SendMessage(Irc.SendType.Message, e.Data.Channel, "Ошибка!");
                }

            }
        }
        #endregion
    }
}
