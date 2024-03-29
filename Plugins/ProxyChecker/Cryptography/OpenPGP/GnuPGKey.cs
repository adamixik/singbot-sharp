/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2012 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Starksoft.Cryptography.OpenPGP
{
    /// <summary>
    /// Class structure that proves a read-only view of the GnuPG keys. 
    /// </summary>
    public class GnuPGKey
    {
        private string _key;
        private DateTime _keyExpiration;
        private string _userId;
        private string _userName;
        private string _subKey;
        private DateTime _subKeyExpiration;
        private string _raw;

        /// <summary>
        /// GnuPGKey constructor.
        /// </summary>
        /// <param name="raw">Raw output stream text data containing key information.</param>
        public GnuPGKey(string raw)
        {
            _raw = raw;
            ParseRaw();          
        }

        /// <summary>
        /// Key text information.
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Key expiration date and time.
        /// </summary>
        public DateTime KeyExpiration
        {
            get { return _keyExpiration; }
        }

        /// <summary>
        /// Key user identification.
        /// </summary>
        public string UserId
        {
            get { return _userId; }
        }

        /// <summary>
        /// Key user name.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }

        /// <summary>
        /// Sub-key information.
        /// </summary>
        public string SubKey
        {
            get { return _subKey; }
        }

        /// <summary>
        /// Sub-key expiration data and time.
        /// </summary>
        public DateTime SubKeyExpiration
        {
            get { return _subKeyExpiration; }
        }

        /// <summary>
        /// Raw output key text generated by GPG.EXE.
        /// </summary>
        public string Raw
        {
            get { return _raw; }
        }

        //sec   1024D/543C3595 2006-12-10
        //uid                  Benton Stark <benton@starksoft.com>
        //uid       ...
        //ssb   1024g/42A71AD8 2006-12-10
        //
        //pub   1024D/543C3595 2006-12-10
        //uid                  Benton Stark <benton@starksoft.com>
        //uid       ...
        //uid       ...
        //sub   1024g/42A71AD8 2006-12-10   
        //
        //pub   1024D/543C3595 2006-12-10
        //uid                  Benton Stark <benton@starksoft.com>
        //uid       ...
        
        private void ParseRaw()
        {
            string[] lines = _raw.Split(new char[] { '\r', '\n' }, 
                            StringSplitOptions.RemoveEmptyEntries);

            string[] pub = SplitSpaces(lines[0]);
            string uid = lines[1];
            string[] sub = SplitSpaces(lines[2]);
                        
            _key = pub[1];
            _keyExpiration = DateTime.Parse(pub[2]);
            _subKey = sub[1];
            _subKeyExpiration = DateTime.Parse(sub[2]);

            ParseUid(uid);
        }

        private string[] SplitSpaces(string input)
        {
            char[] splitChar = { ' '};
            return input.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
        }

     
        private void ParseUid(string uid)
        {
            Regex name = new Regex(@"(?<=uid).*(?=<)");
            Regex userId = new Regex(@"(?<=<).*(?=>)");

            _userName = name.Match(uid).ToString().Trim();
            _userId = userId.Match(uid).ToString();
        }

    }
}
