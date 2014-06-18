'
'SampleVB script for the SingBot IRC Bot [http://singbot.unix-net.ru]
'Copyright (C) 2010 adamix
'
'This program is free software; you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation; either version 2 of the License, or
'(at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.
'
'You should have received a copy of the GNU General Public License
'along with this program; if not, write to the Free Software
'Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'


#Region "Using directives"
Imports System
Imports System.Collections.Generic
Imports System.Threading
#End Region

Namespace SingBot.Scripts
	Public Class Sample
		Inherits Script

		#Region " Constructor/Destructor "
		Public Sub New(bot As Bot)
			MyBase.New(bot)
			Console.WriteLine("Message from SampleVB Script")
		End Sub
		#End Region

		#Region " Methods "
		#End Region

	End Class
End Namespace