using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using remoting_interfaces;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.IO;

namespace pcs
{
    class pcs
    {
        static private pcs_object pcs_obj;

        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, false);

            pcs_obj = new pcs_object();

            RemotingServices.Marshal(pcs_obj, "pcs", typeof(pcs_object));

            System.Console.WriteLine("-pcs-");
            System.Console.WriteLine("<enter> to exit...");
            System.Console.ReadLine();
        }
    }

    public class pcs_object : MarshalByRefObject, Ipcs
    {
        public void create_replica(int rep_factor, string replica_URL, string WhatOperator, int op_id)
        {
         
            string path = Directory.GetCurrentDirectory();
            path = path.Remove(path.Length - 13);
            path = path + @"operator\bin\Debug\@operator.exe"; //acertamos o caminho onde está o executável de operador

            string[] aux = replica_URL.Split(','); // guardamos em aux os URLs
          
            for (int i = 0; i < rep_factor; i++) //para criar o numero de processos especificado por rep_factor
            {
               Process.Start(path, aux[i] + "|" + WhatOperator +"|"+ op_id); //criamos processos operador, passando o seu URL, o tipo de operador com parametros(caso existam) e o id
                                   
             }
            
        }
    }
}