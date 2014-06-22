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
using System.Timers;
#endregion

namespace SingBot.Scripts {

    public class BombCommand : SingBot.Command
    {
        

        BombScript script = null;
        public BombCommand(BombScript script)
            : base("бдыж", "{cmd} [ник]", Permissions.AccessLevel.ACCESS_NULL, CommandType.COMMAND_TYPE_CHANNEL, 1)
        {
            this.script = script;
            Aliases.Add("+бдыж");
            Aliases.Add("+бомба");
            Aliases.Add("!bdyzh");
        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            base.OnCommand(n, e, type, args);

            if (!script.IsChannelEnabled(e.Data.Channel))
            {
                Console.WriteLine("returning...");
                return;
            }

            int time = script.GetTimeBeforeNextUse(e.Data.Nick);
            

            if(time != -1 && script.used[time].seconds > 0)
            {
                n.SendMessage(Irc.SendType.Notice, e.Data.Nick, "Вы уже закладывали бомбу, вы сможете заложить новую через " + Script.FormatBold(script.used[time].seconds.ToString()) + " секунд.");
                return;
            }

            string nick = args[0];

            if(script.IsBombActive(e.Data.Channel, nick))
            {
                n.SendMessage(Irc.SendType.Notice, e.Data.Nick, "Бомба уже в лифчике у " + Script.FormatBold(nick) + "!");
                return;
            }

            var user = n.GetChannelUser(e.Data.Channel, nick);
            if (user == null)
            {
                n.SendMessage(Irc.SendType.Notice, e.Data.Nick, "Пользователя " + Script.FormatBold(nick) + " нет на канале " + Script.FormatBold(e.Data.Channel) + "!");
                return;
            }

            if(n.Nickname == nick)
            {
                n.SendMessage(Irc.SendType.Message, e.Data.Channel, Script.FormatBold(Script.FormatColor(e.Data.Nick, IrcColor.LightRed)) + Script.FormatColor(" совсем охренел и попытался меня взорвать... Но не тут-то было!", IrcColor.Blue));
                n.RfcKick(e.Data.Channel, e.Data.Nick, Script.FormatBold("Ишь чего захотел, совсем обнаглел уже..."));
                return;
            }

            if(e.Data.Nick == nick)
            {
                n.SendMessage(Irc.SendType.Message, e.Data.Channel, Script.FormatColor("Горе-террорист ", IrcColor.Blue) + Script.FormatBold(Script.FormatColor(e.Data.Nick, IrcColor.LightRed)) + Script.FormatColor(" взорвался на своей же бомбе... Вот идиот.", IrcColor.Blue));
                n.RfcKick(e.Data.Channel, e.Data.Nick, Script.FormatBold("БДЫЖ! Горе-террорист подорвался на своей же бомбе..."));
                return;
            }

            Random rand = new Random();
            Bomb b = new Bomb();

            var c = new List<string>();
            string[] array = new string[script.cables.Count];
            script.cables.CopyTo(array);
            foreach (string a in array)
                c.Add(a);

            List<string> curcables = new List<string>();
            int nums = rand.Next(2, c.Count);
            for (int i = 0; i < nums; i++)
            {
                int cable = rand.Next(0, c.Count);
                curcables.Add(c[cable]);
                c.Remove(c[cable]);
            }

            int cur = rand.Next(0, curcables.Count);
            int seconds = rand.Next(21, 58);
            b.cable = curcables[cur];
            b.user = nick;
            Console.WriteLine("Current cable: " + b.cable);
            b.timer = new System.Timers.Timer(seconds * 1000);
            b.channel = e.Data.Channel;

            b.timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
                {
                    n.RfcKick(b.channel, nick, Script.FormatBold("Бабах! Бомба взорвалась!"));
                    b.timer.Stop();
                    if(!script.IsLeft(b.channel, nick))
                        script.bombs.Remove(b);
                };

            string colors = "";
            foreach (string color in curcables)
            {
                switch(color)
                {
                    case "зеленый":
                        {
                            colors += Script.FormatColor(color, IrcColor.Green);
                            break;
                        }
                    case "оранжевый":
                        {
                            colors += Script.FormatColor(color, IrcColor.Orange);
                            break;
                        }
                    case "красный":
                        {
                            colors += Script.FormatColor(color, IrcColor.LightRed);
                            break;
                        }
                    case "синий":
                        {
                            colors += Script.FormatColor(color, IrcColor.Blue);
                            break;
                        }
                    case "фиолетовый":
                        {
                            colors += Script.FormatColor(color, IrcColor.Purple);
                            break;
                        }
                    case "розовый":
                        {
                            colors += Script.FormatColor(color, IrcColor.Pink);
                            break;
                        }
                    case "желтый":
                        {
                            colors += Script.FormatColor(color, IrcColor.Yellow);
                            break;
                        }
                }
                colors += " ";
            }

            n.SendMessage(Irc.SendType.Message, e.Data.Channel, Script.FormatColor(e.Data.Nick, IrcColor.LightRed) + Script.FormatColor(" подкладывает бомбу в лифчик ", IrcColor.Blue) + Script.FormatColor(nick, IrcColor.LightRed) +
                Script.FormatColor("!!! На таймере ", IrcColor.Blue) + Script.FormatColor(Script.FormatUnderlined(seconds.ToString()), IrcColor.LightRed) + Script.FormatColor(" секунд! Всего на бомбе ", IrcColor.Blue) + Script.FormatColor(Script.FormatUnderlined(curcables.Count.ToString()), IrcColor.LightRed) +
                Script.FormatColor(" провода: [ ", IrcColor.Blue) + colors + Script.FormatColor("] ", IrcColor.Blue) + Script.FormatColor(nick, IrcColor.LightRed) + Script.FormatColor(" набери ", IrcColor.Blue) + Script.FormatBold("!куснуть") + Script.FormatColor(" и название цвета что-бы обезвредить бомбу!", IrcColor.Blue));
            b.timer.Start();

            script.bombs.Add(b);

            CMDUsed u = new CMDUsed();
            u.user = e.Data.Nick;
            u.seconds = 60;
            script.used.Add(u);
            System.Timers.Timer timeout = new System.Timers.Timer(1000);
            timeout.Elapsed += (object sender, System.Timers.ElapsedEventArgs ev) =>
                {
                    int i = script.GetTimeBeforeNextUse(e.Data.Nick);
                    script.used[i].seconds -= 1;
                    if (script.used[i].seconds == 0)
                    {
                        timeout.Stop();
                        script.used.RemoveAt(i);
                    }
                };
            timeout.AutoReset = true;
            timeout.Start();
        }
    }

    public class CutCommand : SingBot.Command
    {
        BombScript script = null;
        public CutCommand(BombScript script) : base("куснуть", "{pfx}куснуть [цвет]", Permissions.AccessLevel.ACCESS_NULL, CommandType.COMMAND_TYPE_CHANNEL, 1)
        {
            this.script = script;
            Aliases.Add("!откусить");
            Aliases.Add("!разрезать");
        }

        public override void OnCommand(Network n, Irc.IrcEventArgs e, CommandType type, List<string> args)
        {
            base.OnCommand(n, e, type, args);

            if (!script.IsChannelEnabled(e.Data.Channel)) return;

            string cur = args[0];

            if(!script.IsBombActive(e.Data.Channel, e.Data.Nick))
                return;

            Bomb b = script.GetBomb(e.Data.Channel, e.Data.Nick);
            if(b == null)
                return;

            if(b.cable == cur)
            {
                n.SendMessage(Irc.SendType.Message, e.Data.Channel, Script.FormatBold(Script.FormatColor(e.Data.Nick, IrcColor.Pink) + Script.FormatColor(" успешно обезвредил бомбу!", IrcColor.Green)));
                Cleanup(b);
                return;
            }
            else
            {
                n.SendMessage(Irc.SendType.Message, e.Data.Channel, Script.FormatColor(Script.FormatBold(e.Data.Nick) + " откусил неверный провод..." + Script.FormatBold("БДЫЖ!"), IrcColor.LightRed));
                n.RfcKick(e.Data.Channel, e.Data.Nick, Script.FormatBold("Хреновый из тебя сапер, " + e.Data.Nick));
                Cleanup(b);
                return;
            }
        }
        public void Cleanup(Bomb b)
        {
            b.timer.Stop();
            script.bombs.Remove(b);
        }
    }

    public class Bomb
    {
        public string channel = "";
        public string cable = "";
        public string user = "";
        public System.Timers.Timer timer = null;
        public bool left = false;
    }

    public class CMDUsed
    {
        public string user = "";
        public int seconds;
    }

	public class BombScript : Script {

        public List<string> cables = new List<string>();
        public List<Bomb> bombs = new List<Bomb>();
        public List<CMDUsed> used = new List<CMDUsed>();
		#region " Constructor/Destructor "
        public BombScript(Bot bot)
			: base(bot) {
                cables.Add("зеленый");
                cables.Add("оранжевый");
                cables.Add("красный");
                cables.Add("синий");
                cables.Add("фиолетовый");
                cables.Add("розовый");
                cables.Add("желтый");
                new BombCommand(this);
                new CutCommand(this);
                Bot.OnNickChange += Bot_OnNickChange;
                Bot.OnPart += Bot_OnPart;
                Bot.OnJoin += Bot_OnJoin;
		}
		#endregion

		#region " Methods "
        public bool IsBombActive(string channel, string nick)
        {
            foreach(Bomb b in bombs)
            {
                if (b.channel == channel && b.user == nick)
                    return true;
            }
            return false;
        }

        public Bomb GetBomb(string channel, string nick)
        {
            foreach (Bomb b in bombs)
            {
                if (b.channel == channel && b.user == nick)
                    return b;
            }
            return null;
        }

        public int GetTimeBeforeNextUse(string user)
        {
            if (used.Count == 0)
                return -1;
            for(int i = 0; i < used.Count; i++)
            {
                if (used[i].user == user)
                    return i;
            }
            return -1;
        }

        public bool IsLeft(string channel, string nick)
        {
            Bomb b = GetBomb(channel, nick);
            if(b != null)
            {
                return b.left;
            }
            return false;
        }

		#endregion

        #region " Events "
        void Bot_OnJoin(Network network, Irc.JoinEventArgs e)
        {
            if (IsBombActive(e.Channel, e.Who))
            {
                network.SendMessage(Irc.SendType.Message, e.Channel, FormatColor("Ну что, " + FormatBold(e.Who) + "? Пришел? " + FormatBold("БДЫЖ!"), IrcColor.LightRed));
                network.RfcKick(e.Channel, e.Who, FormatBold("Ну ты же знал, что лучше не бегать..."));
                bombs.Remove(GetBomb(e.Channel, e.Who));
            }
        }

        void Bot_OnPart(Network network, Irc.PartEventArgs e)
        {
            if (IsBombActive(e.Channel, e.Who))
            {
                network.SendMessage(Irc.SendType.Message, e.Channel, FormatColor("Фу, " + FormatBold(e.Who) + " трус и пытается убежать от бомбы... Ну ничего, мы его подождем.", IrcColor.Blue));
                for (int i = 0; i < bombs.Count; i++)
                {
                    if (bombs[i].channel == e.Channel && bombs[i].user == e.Who)
                    {
                        bombs[i].timer.Stop();
                        bombs[i].left = true;
                    }
                }
            }
        }

        void Bot_OnNickChange(Network network, Irc.NickChangeEventArgs e)
        {
            foreach (var chan in network.GetChannels())
            {
                if (IsBombActive(chan, e.OldNickname))
                {
                    network.SendMessage(Irc.SendType.Message, chan, FormatColor("Фу, " + FormatBold(e.NewNickname) + " трус!", IrcColor.LightRed));
                    network.RfcKick(chan, e.NewNickname, FormatBold("От себя не убежишь, от бомбы в лифчике тоже..."));
                    Bomb b = GetBomb(chan, e.OldNickname);
                    if (b != null)
                    {
                        b.timer.Stop();
                        bombs.Remove(b);
                    }
                }
            }
        }
        #endregion
    }
}
