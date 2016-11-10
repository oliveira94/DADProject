using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using INTERFACES;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.IO;

namespace ProcessCreationService
{
    class pcs
    {
        static private pcs_object pcs_obj;

        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, false);

            pcs_obj = new pcs_object();

            RemotingServices.Marshal(pcs_obj, "count_pcs", typeof(pcs_object));

            System.Console.WriteLine("Count_pcs");
            System.Console.WriteLine("<enter> to exit...");
            System.Console.ReadLine();
        }
    }

    public class pcs_object : MarshalByRefObject, Ipcs
    {
        public void create_replica(int rep_factor, string replica_URL, string WhatOperator)
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Remove(path.Length - 20);

            string[] aux = replica_URL.Split(',');
            foreach (string a in aux)
            {
                a.Replace(" ", string.Empty);
            }
            for (int i = 0; i < rep_factor; i++)
            {
                //Process.Start(path + @"\\Count\\bin\\Debug\\Count.exe", aux[i]);
                switch (WhatOperator)
                {
                    case "FILTER":
                        
                        break;
                    case "CUSTOM":
                        
                        break;
                    case "UNIQ":
                        
                        break;
                    case "COUNT":
                        Process.Start(path + @"\\Count\\bin\\Debug\\Count.exe", aux[i]);
                        break;
                }
            }
        }
    }
}
