using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    /// <summary>
    /// Knows the current state of the control data on the remote server
    /// </summary>
    class RemoteControlListener
    {
        private const string URI = "http://www.obscuregame.com:5001/TDV";

        private string[] args = null;

        readonly object locker = new object();

        public RemoteControlListener()
        {
            Thread t = new Thread(update);          // Kick off a new thread
            t.Start();                               // running WriteY()
        }

        void update()
        {
            while (true)
            {
                try
                {
                    StreamReader inStream;
                    WebRequest webRequest;
                    WebResponse webresponse;
                    webRequest = WebRequest.Create(URI);
                    webresponse = webRequest.GetResponse();
                    inStream = new StreamReader(webresponse.GetResponseStream());
                    string s = inStream.ReadToEnd();
                    char[] seps = { ',' };
                    lock (locker)
                    {
                        string[] args = s.Split(seps);
                        if (args.Length != 6)
                            throw new Exception("Wrong format for control string");
                    }

                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
