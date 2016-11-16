using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using remoting_interfaces;

namespace puppet_master
{
    static class Puppet_master
    {
        static string semantics;
        static string loggin_level = "light"; //nivel de logging for defeito
        public delegate void RemoteAsyncDelegate(int rep_factor, string replica_URL, string WhatOperator, int op_id); //irá apontar para a função a ser chamada assincronamente
        static AsyncCallback funcaoCallBack; //irá chamar uma função quando a função assincrona terminar
        static Ipcs pcs_obj;

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

            System.IO.StreamReader file = new System.IO.StreamReader(@"\\Mac\Home\Documents\GitHub\FindSomethingElse\DADSTORM\conf_file.txt"); // Mudar o path para testar

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

                        for (int i = 1; i < op1.rep_factor; i++) //para o apanhar o numero de URLs especificado em rep_factor 
                        {
                            op1.address = op1.address + words[8 + i];
                        }

                        op1.operator_spec = words[8 + op1.rep_factor + 1]; // guardamos o tipo de operador 
                        if (!op1.operator_spec.Equals("COUNT")) // Se o tipo for diferente de "count" significa que ainda falta concatenar os parametros
                        {
                            op1.operator_spec = op1.operator_spec + ":" + words[(8 + op1.rep_factor + 1) + 1]; //concatenamos o tipo de operador com os seus parametros
                        }
                    }
                    else if (words[0] == "OP2")
                    {
                        op2.operator_id = 2;
                        op2.input_ops = words[2];
                        op2.rep_factor = Int32.Parse(words[4]);
                        op2.routing = words[6];
                        op2.address = words[8];

                        for (int i = 1; i < op2.rep_factor; i++) 
                        {
                            op2.address = op2.address + words[8 + i];
                        }

                        op2.operator_spec = words[8 + op2.rep_factor + 1]; 
                        if (!op2.operator_spec.Equals("COUNT"))  
                        {
                            op2.operator_spec = op2.operator_spec + ":" + words[(8 + op2.rep_factor + 1) + 1]; 
                        }
                    }
                    else if (words[0] == "OP3")
                    {
                        op3.operator_id = 3;
                        op3.input_ops = words[2];
                        op3.rep_factor = Int32.Parse(words[4]);
                        op3.routing = words[6];
                        op3.address = words[8];

                        for (int i = 1; i < op2.rep_factor; i++)  
                        {
                            op3.address = op3.address + words[8 + i];
                        }

                        op3.operator_spec = words[8 + op3.rep_factor + 1];
                        if (!op3.operator_spec.Equals("COUNT"))
                        {
                            op3.operator_spec = op3.operator_spec + ":" + words[(8 + op3.rep_factor + 1) + 1];
                        }
                    }
                    else if (words[0] == "OP4")
                    {
                        op4.operator_id = 4;
                        op4.input_ops = words[2];
                        op4.rep_factor = Int32.Parse(words[4]);
                        op4.routing = words[6];
                        op4.address = words[8];
                        for (int i = 1; i < op4.rep_factor; i++) 
                        {
                            op4.address = op4.address + words[8 + i];
                        }

                        op4.operator_spec = words[8 + op4.rep_factor + 1];
                        if (!op4.operator_spec.Equals("COUNT"))
                        {
                            op4.operator_spec = op4.operator_spec + ":" + words[(8 + op4.rep_factor + 1) + 1];
                        }
                    }

                }
            }
        }

        static public void create_replicas()
        {
             pcs_obj = (Ipcs)Activator.GetObject(typeof(Ipcs), "tcp://localhost:10000/pcs");

             funcaoCallBack = new AsyncCallback(OnExit);//aponta para a função de retorno da função assincrona
             RemoteAsyncDelegate dele = new RemoteAsyncDelegate(pcs_obj.create_replica);//aponta para a função a ser chamada assincronamente

             IAsyncResult result = dele.BeginInvoke(op1.rep_factor, op1.address, op1.operator_spec, 1, funcaoCallBack, null);

             result = dele.BeginInvoke(op2.rep_factor, op2.address, op2.operator_spec, 2, funcaoCallBack, null);
             result = dele.BeginInvoke(op3.rep_factor, op3.address, op3.operator_spec, 3, funcaoCallBack, null);
             result = dele.BeginInvoke(op4.rep_factor, op4.address, op4.operator_spec, 4, funcaoCallBack, null);

           
        }
                   
                                             
       

        static public void print_var() // testar se as variáveis foram guardadas de acordo com o ficheiro de configuração
        {
            Console.WriteLine("ALL vars: \r\n");
            Console.WriteLine("-operator_id " + op1.operator_id + " -input:" + op1.input_ops + " -rep_fact:" + op1.rep_factor + " -routing:" + op1.routing + " -address:" + op1.address + " -operator:" + op1.operator_spec +"\r\n");
            Console.WriteLine("-operator_id " + op2.operator_id + " -input:" + op2.input_ops + " -rep_fact:" + op2.rep_factor + " -routing:" + op2.routing + " -address:" + op2.address + " -operator:" + op2.operator_spec + "\r\n");
            Console.WriteLine("-operator_id " + op3.operator_id + " -input:" + op3.input_ops + " -rep_fact:" + op3.rep_factor + " -routing:" + op3.routing + " -address:" + op3.address + " -operator:" + op3.operator_spec + "\r\n");
            Console.WriteLine("-operator_id " + op4.operator_id + " -input:" + op4.input_ops + " -rep_fact:" + op4.rep_factor + " -routing:" + op4.routing + " -address:" + op4.address + " -operator:" + op4.operator_spec + "\r\n");
        }

        public static void OnExit(IAsyncResult ar)
        {
            Console.WriteLine("Function has returned");    
        }


    }

    public class puppet_master_object : MarshalByRefObject, Ipuppet_master
    {


    }

}