using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using INTERFACES;

namespace PUPPET_MASTER
{
    static class Puppet_master
    {

        static string semantics;
        static string loggin_level;
        static List<String> WhatOperator = new List<string>();

        public struct Operator
        {
            public int operator_id;
            public string input_ops;
            public int rep_factor;
            public string routing;
            public string address;
            public string operator_spec;
        }

        static private Operator op1;
        static private Operator op2;
        static private Operator op3;
        static private Operator op4;

        static void Main(string[] args)
        {
            read_conf_file();
            print_var();
            create_replicas();
            Console.ReadLine();
        }

        static void read_conf_file()
        {
            op1 = new Operator();
            op2 = new Operator();
            op3 = new Operator();
            op4 = new Operator();

            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\artur\Documents\GitHub\FindSomethingElse\conf_file.txt"); // Mudar o path para testar
            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(' ');
                foreach (string s in words)
                {
                    if (words[0] == "Semantics")
                    {
                        semantics = words[1];
                    }
                    else if (words[0] == "LoggingLevel")
                    {
                        loggin_level = words[1];
                    }
                    else if (words[0] == "OP1")
                    {
                        op1.operator_id = 1;
                        op1.input_ops = words[2];
                        op1.rep_factor = Int32.Parse(words[4]);
                        op1.routing = words[6];
                        op1.address = words[8];
                        for (int i = 1; i < op1.rep_factor; i++) // for para o apanhar o numero de URLs especificado em rep_factor (neste exemplo são 2)
                        {
                            op1.address = op1.address + words[8 + i];
                        }
                        WhatOperator.Add(words[8 + op1.rep_factor + 1]);
                        break;

                        //op1.operator_id = 1;
                        //op1.input_ops = words[3];
                        //op1.rep_factor = Int32.Parse(words[6]);
                        //op1.routing = words[8];
                        //op1.address = words[10];
                        //for (int i = 1; i < op1.rep_factor; i++) // for para o apanhar o numero de URLs especificado em rep_factor (neste exemplo são 2)
                        //{
                        //    op1.address = op1.address + words[8 + i];
                        //}
                        //WhatOperator = words[8 + op1.rep_factor + 1];
                    }
                    else if (words[0] == "OP2")
                    {
                        op2.operator_id = 2;
                        op2.input_ops = words[2];
                        op2.rep_factor = Int32.Parse(words[4]);
                        op2.routing = words[6];
                        op2.address = words[8];
                        for (int i = 1; i < op2.rep_factor; i++) // for para o apanhar o numero de URLs especificado em rep_factor 
                        {
                            op2.address = op2.address + words[8 + i];
                        }
                        WhatOperator.Add(words[8 + op2.rep_factor + 1]);
                        break;
                    }
                    else if (words[0] == "OP3")
                    {
                        op3.operator_id = 3;
                        op3.input_ops = words[2];
                        op3.rep_factor = Int32.Parse(words[4]);
                        op3.routing = words[6];
                        op3.address = words[8];
                        for (int i = 1; i < op2.rep_factor; i++) // for para o apanhar o numero de URLs especificado em rep_factor 
                        {
                            op3.address = op3.address + words[8 + i];
                        }
                        WhatOperator.Add(words[8 + op3.rep_factor + 1]);
                        break;
                    }
                    else if (words[0] == "OP4")
                    {
                        op4.operator_id = 4;
                        op4.input_ops = words[2];
                        op4.rep_factor = Int32.Parse(words[4]);
                        op4.routing = words[6];
                        op4.address = words[8];
                        for (int i = 1; i < op4.rep_factor; i++) // for para o apanhar o numero de URLs especificado em rep_factor 
                        {
                            op4.address = op4.address + words[8 + i];
                        }
                        WhatOperator.Add(words[8 + op4.rep_factor + 1]);
                        break;
                    }

                }
            }
        }

        static public void create_replicas()
        {
            Ipcs pcs_obj = (Ipcs)Activator.GetObject(typeof(Ipcs), "tcp://localhost:10000/count_pcs");
            //pcs_obj.create_replica(op4.rep_factor, op4.address);

            for (int i = 0; i < WhatOperator.Count; i++)
            {
                switch (WhatOperator[i])
                {
                    case "FILTER":
                        pcs_obj.create_replica(op1.rep_factor, op1.address, WhatOperator[i]);
                        break;
                    case "CUSTOM":
                        pcs_obj.create_replica(op2.rep_factor, op2.address, WhatOperator[i]);
                        break;
                    case "UNIQ":
                        pcs_obj.create_replica(op3.rep_factor, op3.address, WhatOperator[i]);
                        break;
                    case "COUNT":
                        pcs_obj.create_replica(op4.rep_factor, op4.address, WhatOperator[i]);
                        break;
                }
            }
        }

        static public void print_var()
        {

            Console.WriteLine("ALL VARS: -operator_id " + op1.operator_id + " -input:" + op1.input_ops + " -rep_fact:" + op1.rep_factor + " -routing:" + op1.routing + " -address:" + op1.address + " -operator:work in progressc");
        }


    }

    public class puppet_master_object : MarshalByRefObject, Ipuppet_master
    {


    }

}