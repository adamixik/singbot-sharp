/**
 * $Id: Exceptions.cs 140 2004-11-30 19:23:58Z meebey $
 * $URL: svn://svn.qnetp.net/smartirc/SmartIrc4net/tags/0.3.5/src/Exceptions.cs $
 * $Rev: 140 $
 * $Author: meebey $
 * $Date: 2004-11-30 20:23:58 +0100 (Tue, 30 Nov 2004) $
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2004 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 * 
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Runtime.Serialization;

namespace SingBot.Irc
{
    [Serializable()]
    public class SmartIrc4netException : ApplicationException
    {
        public SmartIrc4netException() : base()
        {
        }
        
        public SmartIrc4netException(string message) : base(message)
        {
        }
        
        public SmartIrc4netException(string message, Exception e) : base(message, e)
        {
        }
        
        protected SmartIrc4netException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    
    [Serializable()]
    public class ConnectionException : SmartIrc4netException
    {
        public ConnectionException() : base()
        {
        }
        
        public ConnectionException(string message) : base(message)
        {
        }
        
        public ConnectionException(string message, Exception e) : base(message, e)
        {
        }
        
        protected ConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    
    [Serializable()]
    public class CouldNotConnectException : ConnectionException
    {
        public CouldNotConnectException() : base()
        {
        }
        
        public CouldNotConnectException(string message) : base(message)
        {
        }
        
        public CouldNotConnectException(string message, Exception e) : base(message, e)
        {
        }

        protected CouldNotConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable()]
    public class NotConnectedException : ConnectionException
    {
        public NotConnectedException() : base()
        {
        }
        
        public NotConnectedException(string message) : base(message)
        {
        }
        
        public NotConnectedException(string message, Exception e) : base(message, e)
        {
        }
        
        protected NotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable()]
    public class AlreadyConnectedException : ConnectionException
    {
        public AlreadyConnectedException() : base()
        {
        }
        
        public AlreadyConnectedException(string message) : base(message)
        {
        }
        
        public AlreadyConnectedException(string message, Exception e) : base(message, e)
        {
        }
        
        protected AlreadyConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
}
}
