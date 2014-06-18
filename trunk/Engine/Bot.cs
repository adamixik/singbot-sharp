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
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;


namespace SingBot {
	public class Bot : IDisposable {

		private List<string> referencedAssemblies;
		private List<string> load_plugins;
		private List<string> load_scripts;
		private static Bot _this = null;
		
		private readonly string _prefix = "!";
		
		private Permissions Permissions = null;
		
        public class ScriptDomain
        {
            public string file;
            public string name;
            public AppDomain domain;
            public Assembly assembly;
        }

		#region " Constructor/Destructor/Dispose "
		public Bot ()
		{
			#region " Header "
			Console.WriteLine ("SingBot: The C# IRC Bot  - v" + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + " - [http://singbot.unix-net.ru]");
			Console.WriteLine ("(c) 2010-2014 The SingBot Project");
			Console.WriteLine ("===============================================================================");
			Console.WriteLine ("SingBot: The C# IRC Bot made by UNIX-Net workgroup.");
			Console.WriteLine ("===============================================================================");
			#endregion

			referencedAssemblies = new List<string> ();
			load_plugins = new List<string> ();
			load_scripts = new List<string> ();
			Permissions = new Permissions (); 
			_this = this;
			
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler (AssemblyResolveHandler);
			
			#region " Load Configuration "
			if (!File.Exists ("Configuration.xml")) {
				Console.WriteLine ("Config file 'Configuration.xml' not found!");
				Environment.Exit (0);
			}
			configuration = new XmlDocument ();
			configuration.Load ("Configuration.xml");
			foreach (XmlElement e in configuration.GetElementsByTagName("Network")) {
				Network n = new Network ();
				networks.Add (n);
				n.Name = e.Attributes ["Name"].Value;
				n.Nickname = e.Attributes ["Nickname"].Value;
				n.Realname = e.Attributes ["Realname"].Value;
				n.Username = e.Attributes ["Username"].Value;
				if (e.HasAttribute ("Password")) {
					n.UsePassword = true;
					n.Password = e.Attributes ["Password"].Value;
				} else
					n.UsePassword = false;
				n.Port = int.Parse (e.Attributes ["Port"].Value);
				n.SendDelay = int.Parse (e.Attributes ["SendDelay"].Value);

				foreach (XmlElement f in e.GetElementsByTagName("Server"))
					n.Servers.Add (f.Attributes ["Address"].Value);

				foreach (XmlElement f in e.GetElementsByTagName("Channel"))
					n.Channels.Add (f.Attributes ["Name"].Value);

				n.OnBan += new Irc.BanEventHandler (OnBanHandler);
				n.OnChannelAction += new Irc.ActionEventHandler (OnChannelActionHandler);
				n.OnChannelActiveSynced += new Irc.IrcEventHandler (OnChannelActiveSyncedHandler);
				n.OnChannelMessage += new Irc.IrcEventHandler (OnChannelMessageHandler);
				n.OnChannelModeChange += new Irc.IrcEventHandler (OnChannelModeChangeHandler);
				n.OnChannelNotice += new Irc.IrcEventHandler (OnChannelNoticeHandler);
				n.OnChannelPassiveSynced += new Irc.IrcEventHandler (OnChannelPassiveSyncedHandler);
				n.OnConnected += new EventHandler (OnConnectedHandler);
				n.OnConnecting += new EventHandler (OnConnectingHandler);
				n.OnConnectionError += new EventHandler (OnConnectionErrorHandler);
				n.OnCtcpReply += new Irc.IrcEventHandler (OnCtcpReplyHandler);
				n.OnCtcpRequest += new Irc.IrcEventHandler (OnCtcpRequestHandler);
				n.OnDehalfop += new Irc.DehalfopEventHandler (OnDehalfopHandler);
				n.OnDeop += new Irc.DeopEventHandler (OnDeopHandler);
				n.OnDevoice += new Irc.DevoiceEventHandler (OnDevoiceHandler);
				n.OnDisconnected += new EventHandler (OnDisconnectedHandler);
				n.OnDisconnecting += new EventHandler (OnDisconnectingHandler);
				n.OnError += new Irc.ErrorEventHandler (OnErrorHandler);
				n.OnErrorMessage += new Irc.IrcEventHandler (OnErrorMessageHandler);
				n.OnHalfop += new Irc.HalfopEventHandler (OnHalfopHandler);
				n.OnInvite += new Irc.InviteEventHandler (OnInviteHandler);
				n.OnJoin += new Irc.JoinEventHandler (OnJoinHandler);
				n.OnKick += new Irc.KickEventHandler (OnKickHandler);
				n.OnModeChange += new Irc.IrcEventHandler (OnModeChangeHandler);
				n.OnMotd += new Irc.MotdEventHandler (OnMotdHandler);
				n.OnNames += new Irc.NamesEventHandler (OnNamesHandler);
				n.OnNickChange += new Irc.NickChangeEventHandler (OnNickChangeHandler);
				n.OnOp += new Irc.OpEventHandler (OnOpHandler);
				n.OnPart += new Irc.PartEventHandler (OnPartHandler);
				n.OnPing += new Irc.PingEventHandler (OnPingHandler);
				n.OnQueryAction += new Irc.ActionEventHandler (OnQueryActionHandler);
				n.OnQueryMessage += new Irc.IrcEventHandler (OnQueryMessageHandler);
				n.OnQueryNotice += new Irc.IrcEventHandler (OnQueryNoticeHandler);
				n.OnQuit += new Irc.QuitEventHandler (OnQuitHandler);
				n.OnRawMessage += new Irc.IrcEventHandler (OnRawMessageHandler);
				n.OnReadLine += new Irc.ReadLineEventHandler (OnReadLineHandler);
				n.OnRegistered += new EventHandler (OnRegisteredHandler);
				n.OnTopic += new Irc.TopicEventHandler (OnTopicHandler);
				n.OnTopicChange += new Irc.TopicChangeEventHandler (OnTopicChangeHandler);
				n.OnUnban += new Irc.UnbanEventHandler (OnUnbanHandler);
				n.OnUserModeChange += new Irc.IrcEventHandler (OnUserModeChangeHandler);
				n.OnVoice += new Irc.VoiceEventHandler (OnVoiceHandler);
				n.OnWho += new Irc.WhoEventHandler (OnWhoHandler);
				n.OnWriteLine += new Irc.WriteLineEventHandler (OnWriteLineHandler);
			}
			foreach (XmlElement f in configuration.GetElementsByTagName("Reference"))
				referencedAssemblies.Add (f.Attributes ["Assembly"].Value);
				
			foreach (XmlElement f in configuration.GetElementsByTagName("Plugin"))
				load_plugins.Add (f.Attributes ["Name"].Value);

			#endregion
            
			#region " Load Plugins "
			foreach (string plugin in load_plugins) {
				LoadPlugin ("Plugins/" + plugin);
			}
			Console.WriteLine ("===============================================================================");
			#endregion

			#region " Load Scripts "
            LoadScripts();
			Console.WriteLine ("===============================================================================");
			#endregion
		}

        public void LoadScripts()
        {
            for (int i = 0; i < scripts.Count; i++ )
            {
                Console.WriteLine("Unloading script...");
                scripts[i].Dispose();
                scripts.RemoveAt(i);
                GC.Collect();
            }
            load_scripts.Clear();
            foreach (XmlElement f in configuration.GetElementsByTagName("Script"))
            {
                load_scripts.Add(f.Attributes["Name"].Value);
                Console.WriteLine("Adding script: " + f.Attributes["Name"].Value);
            }

            foreach (string script in load_scripts)
            {
                LoadScript("Scripts/" + script);
            }
        }

		~Bot() {
			Dispose();
		}

		public void Dispose() {
			DisconnectAll();
		}
		#endregion

		#region " Connect/Disconnect "
		public void ConnectAll() {
			foreach (Network n in Networks)
				n.Connect();
		}

		public void DisconnectAll() {
			foreach (Network n in Networks)
				n.Disconnect();
		}
		#endregion

		#region " Methods "
		public Network GetNetworkByName(string name) {
			foreach (Network n in networks)
				if (n.Name == name)
					return n;
			throw new NetworkNotFoundException();
		}

		public void SaveConfiguration() {
			configuration.Save("Configuration.xml");
		}

        public Plugin LoadPlugin(string path)
        {
            object[] o = { this };
            FileInfo f = new FileInfo(path);
            Console.WriteLine("Loading plugin: '" + f.Name + "' ...");
            Assembly a = System.Reflection.Assembly.LoadFile(f.FullName);
            foreach (Type t in a.GetTypes())
                if (t.BaseType == typeof(Plugin))
                {
                    Plugin p = (Plugin)Activator.CreateInstance(t, o);
                    plugins.Add(p);
                    return p;
                }
            return null;
        }
		
		public Assembly AssemblyResolveHandler (object source, ResolveEventArgs e)
		{
			// you may not want to use First() here, consider FirstOrDefault() as well
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies ())
			{
              	if( asm.GetName ().FullName == e.Name)
					return asm;
			}
			return null;
			//Console.WriteLine ("Resolving: {0}...", e.Name);
			//return Assembly.LoadFrom ("Plugins/" + e.Name);
		}
		
        public Script LoadScript(string path)
        {
            object[] o = { this };
            string file = Path.GetTempFileName();
            FileInfo f = new FileInfo(path);
            if (f.Extension == ".cs")
            {
                if (!CompileCSharpScript(f.FullName, file)) return null; 
            }
            else if (f.Extension == ".vb")
            {
                if (!CompileVBScript(f.FullName, file)) return null;
            }
            else
            {
                return null;
            }

            Assembly a = System.Reflection.Assembly.LoadFile(file + ".dll");
            foreach (Type t in a.GetTypes())
                if (t.BaseType == typeof(Script))
                {
                    Script s = (Script)Activator.CreateInstance(t, o);
                    scripts.Add(s);
                    return s;
                }
            return null;
        }

        Assembly domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Dep: " + args.Name);
            return null;
        }

        public bool CompileCSharpScript (string path, string file)
		{
			FileInfo f = new FileInfo (path);
			Console.WriteLine ("Compiling script: '" + f.Name + "' ...");
			string code = File.ReadAllText (f.FullName);
			Dictionary<string, string> providerOptions = new Dictionary<string, string> () 
			{
                      {"CompilerVersion", "v3.5"}
            };
			CSharpCodeProvider provider = new CSharpCodeProvider (providerOptions);
			CompilerParameters compilerParams = new CompilerParameters () { OutputAssembly = file + ".dll", GenerateExecutable = false };
			compilerParams.ReferencedAssemblies.Add ("System.dll");
			compilerParams.ReferencedAssemblies.Add ("System.Xml.dll");
			compilerParams.ReferencedAssemblies.Add ("System.Data.dll");
			compilerParams.ReferencedAssemblies.Add ("SingBot.Engine.dll");
			compilerParams.ReferencedAssemblies.Add ("SingBot.Util.dll");
			foreach (string reference in referencedAssemblies)
				compilerParams.ReferencedAssemblies.Add (reference);
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, code);
            foreach (CompilerError err in results.Errors)
            {
                Console.WriteLine("Error when compiling script {0} [{1}]: {2}", f.Name, err.Line, err.ErrorText);
                return false;
            }
            return true;
        }

        public bool CompileVBScript (string path, string file)
		{
			FileInfo f = new FileInfo (path);
			Console.WriteLine ("Compiling script: '" + f.Name + "' ...");
			string code = File.ReadAllText (f.FullName);
			Dictionary<string, string> providerOptions = new Dictionary<string, string>
                    {
                      {"CompilerVersion", "v3.5"}
                    };
			VBCodeProvider provider = new VBCodeProvider (providerOptions);
            CompilerParameters compilerParams = new CompilerParameters { OutputAssembly = file + ".dll", GenerateExecutable = false };
			compilerParams.ReferencedAssemblies.Add ("System.dll");
			compilerParams.ReferencedAssemblies.Add ("System.Xml.dll");
			compilerParams.ReferencedAssemblies.Add ("System.Data.dll");
			compilerParams.ReferencedAssemblies.Add ("SingBot.Engine.dll");
			compilerParams.ReferencedAssemblies.Add ("SingBot.Util.dll");
			foreach (string reference in referencedAssemblies)
				compilerParams.ReferencedAssemblies.Add (reference);
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, code);
            foreach (CompilerError err in results.Errors)
            {
                Console.WriteLine("Error when compiling script {0} [{1}]: {2}", f.Name, err.Line,err.ErrorText);
                return false;
            }
            return true;
        }

		#endregion

		#region " Properties "
		XmlDocument configuration;
		public XmlElement Configuration {
			get {
				return configuration["SingBot"];
			}
		}

		List<Network> networks = new List<Network>();
		public List<Network> Networks {
			get {
				return networks;
			}
		}

		List<Plugin> plugins = new List<Plugin>();
		public List<Plugin> Plugins {
			get {
				return plugins;
			}
		}

        List<Script> scripts = new List<Script>();
        public List<Script> Scripts
        {
            get
            {
                return scripts;
            }
        }
		
		public static Bot GetSingleton ()
		{
			return _this;
		}
		
		public Permissions GetPermissions ()
		{
			return Permissions;
		}
		
		public string GetCommandPrefix ()
		{
			return _prefix;
		}
		#endregion

		#region " Global Event Handles "
		void OnBanHandler(object sender, Irc.BanEventArgs e) {
			if (OnBan != null)
				OnBan((Network)sender, e);
		}

		void OnChannelActionHandler(object sender, Irc.ActionEventArgs e) {
			if (OnChannelAction != null)
				OnChannelAction((Network)sender, e);
		}

		void OnChannelActiveSyncedHandler(object sender, Irc.IrcEventArgs e) {
			if (OnChannelActiveSynced != null)
				OnChannelActiveSynced((Network)sender, e);
		}

		void OnChannelMessageHandler(object sender, Irc.IrcEventArgs e) {
			if (OnChannelMessage != null)
				OnChannelMessage((Network)sender, e);
		}

		void OnChannelModeChangeHandler(object sender, Irc.IrcEventArgs e) {
			if (OnChannelModeChange != null)
				OnChannelModeChange((Network)sender, e);
		}

		void OnChannelNoticeHandler(object sender, Irc.IrcEventArgs e) {
			if (OnChannelNotice != null)
				OnChannelNotice((Network)sender, e);
		}

		void OnChannelPassiveSyncedHandler(object sender, Irc.IrcEventArgs e) {
			if (OnChannelPassiveSynced != null)
				OnChannelPassiveSynced((Network)sender, e);
		}

		void OnConnectedHandler(object sender, EventArgs e) {
			if (OnConnected != null)
				OnConnected((Network)sender, e);
		}

		void OnConnectingHandler(object sender, EventArgs e) {
			if (OnConnecting != null)
				OnConnecting((Network)sender, e);
		}

		void OnConnectionErrorHandler(object sender, EventArgs e) {
			if (OnConnectionError != null)
				OnConnectionError((Network)sender, e);
		}

		void OnCtcpReplyHandler(object sender, Irc.IrcEventArgs e) {
			if (OnCtcpReply != null)
				OnCtcpReply((Network)sender, e);
		}

		void OnCtcpRequestHandler(object sender, Irc.IrcEventArgs e) {
			if (OnCtcpRequest != null)
				OnCtcpRequest((Network)sender, e);
		}

		void OnDehalfopHandler(object sender, Irc.DehalfopEventArgs e) {
			if (OnDehalfop != null)
				OnDehalfop((Network)sender, e);
		}

		void OnDeopHandler(object sender, Irc.DeopEventArgs e) {
			if (OnDeop != null)
				OnDeop((Network)sender, e);
		}

		void OnDevoiceHandler(object sender, Irc.DevoiceEventArgs e) {
			if (OnDevoice != null)
				OnDevoice((Network)sender, e);
		}

		void OnDisconnectedHandler(object sender, EventArgs e) {
			if (OnDisconnected != null)
				OnDisconnected((Network)sender, e);
		}

		void OnDisconnectingHandler(object sender, EventArgs e) {
			if (OnDisconnecting != null)
				OnDisconnecting((Network)sender, e);
		}

		void OnErrorHandler(object sender, Irc.ErrorEventArgs e) {
			if (OnError != null)
				OnError((Network)sender, e);
		}

		void OnErrorMessageHandler(object sender, Irc.IrcEventArgs e) {
			if (OnErrorMessage != null)
				OnErrorMessage((Network)sender, e);
		}

		void OnHalfopHandler(object sender, Irc.HalfopEventArgs e) {
			if (OnHalfop != null)
				OnHalfop((Network)sender, e);
		}

		void OnInviteHandler(object sender, Irc.InviteEventArgs e) {
			if (OnInvite != null)
				OnInvite((Network)sender, e);
		}

		void OnJoinHandler(object sender, Irc.JoinEventArgs e) {
			if (OnJoin != null)
				OnJoin((Network)sender, e);
		}

		void OnKickHandler(object sender, Irc.KickEventArgs e) {
			if (OnKick != null)
				OnKick((Network)sender, e);
		}

		void OnModeChangeHandler(object sender, Irc.IrcEventArgs e) {
			if (OnModeChange != null)
				OnModeChange((Network)sender, e);
		}

		void OnMotdHandler(object sender, Irc.MotdEventArgs e) {
			if (OnMotd != null)
				OnMotd((Network)sender, e);
		}

		void OnNamesHandler(object sender, Irc.NamesEventArgs e) {
			if (OnNames != null)
				OnNames((Network)sender, e);
		}

		void OnNickChangeHandler(object sender, Irc.NickChangeEventArgs e) {
			if (OnNickChange != null)
				OnNickChange((Network)sender, e);
		}

		void OnOpHandler(object sender, Irc.OpEventArgs e) {
			if (OnOp != null)
				OnOp((Network)sender, e);
		}

		void OnPartHandler(object sender, Irc.PartEventArgs e) {
			if (OnPart != null)
				OnPart((Network)sender, e);
		}

		void OnPingHandler(object sender, Irc.PingEventArgs e) {
			if (OnPing != null)
				OnPing((Network)sender, e);
		}

		void OnQueryActionHandler(object sender, Irc.ActionEventArgs e) {
			if (OnQueryAction != null)
				OnQueryAction((Network)sender, e);
		}

		void OnQueryMessageHandler(object sender, Irc.IrcEventArgs e) {
			if (OnQueryMessage != null)
				OnQueryMessage((Network)sender, e);
		}

		void OnQueryNoticeHandler(object sender, Irc.IrcEventArgs e) {
			if (OnQueryNotice != null)
				OnQueryNotice((Network)sender, e);
		}

		void OnQuitHandler(object sender, Irc.QuitEventArgs e) {
			if (OnQuit != null)
				OnQuit((Network)sender, e);
		}

		void OnRawMessageHandler(object sender, Irc.IrcEventArgs e) {
			if (OnRawMessage != null)
				OnRawMessage((Network)sender, e);
		}

		void OnReadLineHandler(object sender, Irc.ReadLineEventArgs e) {
			if (OnReadLine != null)
				OnReadLine((Network)sender, e);
		}

		void OnRegisteredHandler(object sender, EventArgs e) {
			if (OnRegistered != null)
				OnRegistered((Network)sender, e);
		}

		void OnTopicHandler(object sender, Irc.TopicEventArgs e) {
			if (OnTopic != null)
				OnTopic((Network)sender, e);
		}

		void OnTopicChangeHandler(object sender, Irc.TopicChangeEventArgs e) {
			if (OnTopicChange != null)
				OnTopicChange((Network)sender, e);
		}

		void OnUnbanHandler(object sender, Irc.UnbanEventArgs e) {
			if (OnUnban != null)
				OnUnban((Network)sender, e);
		}

		void OnUserModeChangeHandler(object sender, Irc.IrcEventArgs e) {
			if (OnUserModeChange != null)
				OnUserModeChange((Network)sender, e);
		}

		void OnVoiceHandler(object sender, Irc.VoiceEventArgs e) {
			if (OnVoice != null)
				OnVoice((Network)sender, e);
		}

		void OnWhoHandler(object sender, Irc.WhoEventArgs e) {
			if (OnWho != null)
				OnWho((Network)sender, e);
		}

		void OnWriteLineHandler(object sender, Irc.WriteLineEventArgs e) {
			if (OnWriteLine != null)
				OnWriteLine((Network)sender, e);
		}
		#endregion

		#region " Events "
		public event EventHandler OnRegistered;
		public event PingEventHandler OnPing;
		public event IrcEventHandler OnRawMessage;
		public event ErrorEventHandler OnError;
		public event IrcEventHandler OnErrorMessage;
		public event JoinEventHandler OnJoin;
		public event NamesEventHandler OnNames;
		public event PartEventHandler OnPart;
		public event QuitEventHandler OnQuit;
		public event KickEventHandler OnKick;
		public event InviteEventHandler OnInvite;
		public event BanEventHandler OnBan;
		public event UnbanEventHandler OnUnban;
		public event OpEventHandler OnOp;
		public event DeopEventHandler OnDeop;
		public event HalfopEventHandler OnHalfop;
		public event DehalfopEventHandler OnDehalfop;
		public event VoiceEventHandler OnVoice;
		public event DevoiceEventHandler OnDevoice;
		public event WhoEventHandler OnWho;
		public event MotdEventHandler OnMotd;
		public event TopicEventHandler OnTopic;
		public event TopicChangeEventHandler OnTopicChange;
		public event NickChangeEventHandler OnNickChange;
		public event IrcEventHandler OnModeChange;
		public event IrcEventHandler OnUserModeChange;
		public event IrcEventHandler OnChannelModeChange;
		public event IrcEventHandler OnChannelMessage;
		public event ActionEventHandler OnChannelAction;
		public event IrcEventHandler OnChannelNotice;
		public event IrcEventHandler OnChannelActiveSynced;
		public event IrcEventHandler OnChannelPassiveSynced;
		public event IrcEventHandler OnQueryMessage;
		public event ActionEventHandler OnQueryAction;
		public event IrcEventHandler OnQueryNotice;
		public event IrcEventHandler OnCtcpRequest;
		public event IrcEventHandler OnCtcpReply;
		public event ReadLineEventHandler OnReadLine;
		public event WriteLineEventHandler OnWriteLine;
		public event EventHandler OnConnecting;
		public event EventHandler OnConnected;
		public event EventHandler OnDisconnecting;
		public event DisconnectedEventHandler OnDisconnected;
		public event EventHandler OnConnectionError;
		#endregion

	}

	#region " Delegates "
	public delegate void IrcEventHandler(Network network, Irc.IrcEventArgs e);
	public delegate void ActionEventHandler(Network network, Irc.ActionEventArgs e);
	public delegate void ErrorEventHandler(Network network, Irc.ErrorEventArgs e);
	public delegate void PingEventHandler(Network network, Irc.PingEventArgs e);
	public delegate void KickEventHandler(Network network, Irc.KickEventArgs e);
	public delegate void JoinEventHandler(Network network, Irc.JoinEventArgs e);
	public delegate void NamesEventHandler(Network network, Irc.NamesEventArgs e);
	public delegate void PartEventHandler(Network network, Irc.PartEventArgs e);
	public delegate void InviteEventHandler(Network network, Irc.InviteEventArgs e);
	public delegate void OpEventHandler(Network network, Irc.OpEventArgs e);
	public delegate void DeopEventHandler(Network network, Irc.DeopEventArgs e);
	public delegate void HalfopEventHandler(Network network, Irc.HalfopEventArgs e);
	public delegate void DehalfopEventHandler(Network network, Irc.DehalfopEventArgs e);
	public delegate void VoiceEventHandler(Network network, Irc.VoiceEventArgs e);
	public delegate void DevoiceEventHandler(Network network, Irc.DevoiceEventArgs e);
	public delegate void BanEventHandler(Network network, Irc.BanEventArgs e);
	public delegate void UnbanEventHandler(Network network, Irc.UnbanEventArgs e);
	public delegate void TopicEventHandler(Network network, Irc.TopicEventArgs e);
	public delegate void TopicChangeEventHandler(Network network, Irc.TopicChangeEventArgs e);
	public delegate void NickChangeEventHandler(Network network, Irc.NickChangeEventArgs e);
	public delegate void QuitEventHandler(Network network, Irc.QuitEventArgs e);
	public delegate void WhoEventHandler(Network network, Irc.WhoEventArgs e);
	public delegate void MotdEventHandler(Network network, Irc.MotdEventArgs e);
	public delegate void ReadLineEventHandler(Network network, Irc.ReadLineEventArgs e);
	public delegate void WriteLineEventHandler(Network network, Irc.WriteLineEventArgs e);
	public delegate void DisconnectedEventHandler(Network network, EventArgs e);
	#endregion

    #region " IRC Colors "
    public enum IrcColor
    {
        White = 00,
        Black = 01,
        Blue = 02,
        Green = 03,
        LightRed = 04,
        Brown = 05,
        Purple = 06,
        Orange = 07,
        Yellow = 08,
        LightGreen = 09,
        Cyan = 10,
        LightCyan = 11,
        LightBlue = 12,
        Pink = 13,
        Grey = 14,
        LightGrey = 15
    };
    #endregion
}
