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

namespace COUNT_PCS
{
    class count_pcs
    {
        static private count_pcs_object count_pcs_obj;

        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, false);

            count_pcs_obj = new count_pcs_object();

            RemotingServices.Marshal(count_pcs_obj, "count_pcs", typeof(count_pcs_object));

            System.Console.WriteLine("Count_pcs");
            System.Console.WriteLine("<enter> to exit...");
            System.Console.ReadLine();
        } 
    }

    public class count_pcs_object : MarshalByRefObject, Icount_pcs
    {
         public void create_replica(int rep_factor, string replica_URL)
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
                 Process.Start(path + @"\\Count\\bin\\Debug\\Count.exe", aux[i]); 
             }

            
             
        }
    }
}
