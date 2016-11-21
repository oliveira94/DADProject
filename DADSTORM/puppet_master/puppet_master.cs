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
        public delegate void RemoteAsyncDelegate(int rep_factor, string replica_URL, string whatoperator, string op_id); //irá apontar para a função a ser chamada assincronamente
        static AsyncCallback funcaoCallBack; //irá chamar uma função quando a função assincrona terminar
        static Ipcs pcs_obj;
        static IOperator op_obj1;
        static IOperator op_obj2;
        static IOperator op_obj3;
        static IOperator op_obj4;
  
        public struct Operator
        {
            public string operator_id;
            public string input_ops;
            public int rep_factor;
            public string routing;
            public string address;
            public string operator_spec;
        }

        static private Operator op1; //guardar estes operadores em List<Operator> fica dinamico
        static private Operator op2;
        static private Operator op3;
        static private Operator op4;

        static void Main(string[] args)
        {
           
            read_conf_file();
            create_replicas();
            next_Operator();
            Console.WriteLine("Input (start OP{1-4}) command");
            while(true)
            {
                string command = Console.ReadLine();
                read_command(command);
            }       
        }
      

        static void read_conf_file()
        {
            op1 = new Operator();
            op2 = new Operator();
            op3 = new Operator();
            op4 = new Operator();

            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\lj0se\Documents\GitHub\FindSomethingElse\DADSTORM\conf_file.txt"); // Mudar o path para testar

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
                        op1.operator_id = "OP1";
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
                        op2.operator_id = "OP2";
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
                        op3.operator_id = "OP3";
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
                        op4.operator_id = "OP4";
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
            //(int rep_factor, string replica_URL, string whatoperator, string op_id)
             funcaoCallBack = new AsyncCallback(OnExit);//aponta para a função de retorno da função assincrona
             RemoteAsyncDelegate dele = new RemoteAsyncDelegate(pcs_obj.create_replica);//aponta para a função a ser chamada assincronamente

             IAsyncResult result = dele.BeginInvoke(op1.rep_factor, op1.address, op1.operator_spec, op1.operator_id, funcaoCallBack, null);

             result = dele.BeginInvoke(op2.rep_factor, op2.address, op2.operator_spec, op2.operator_id, funcaoCallBack, null);
             result = dele.BeginInvoke(op3.rep_factor, op3.address, op3.operator_spec, op3.operator_id, funcaoCallBack, null);
             result = dele.BeginInvoke(op4.rep_factor, op4.address, op4.operator_spec, op4.operator_id, funcaoCallBack, null);         
        }
                   
        static public void next_Operator() //envia informação a cada operador sobre o operador seguinte
        {
          op_obj1 = (IOperator)Activator.GetObject(typeof(IOperator), routing(op1.operator_id));//cria um objeto remoto em OP1
          op_obj1.next_op(routing(op2.operator_id), op2.operator_spec);                         //envia o URL e o tipo de operador do OP2 para OP1 

          op_obj2 = (IOperator)Activator.GetObject(typeof(IOperator), routing(op2.operator_id));
          op_obj2.next_op(routing(op3.operator_id), op3.operator_spec);

          op_obj3 = (IOperator)Activator.GetObject(typeof(IOperator), routing(op3.operator_id));
          op_obj3.next_op(routing(op4.operator_id), op4.operator_spec);

          op_obj4 = (IOperator)Activator.GetObject(typeof(IOperator), routing(op4.operator_id));
          op_obj4.next_op("nulo", "nulo");                       
         }
  

        private static string routing(string op_id)         //terá um argumento para o tipo de routing (ainda está em primary)
        {
            string[] words = op1.address.Split(',');       // contem todos os URls do OP1   
            string[] words2 = op2.address.Split(',');          
            string[] words3 = op3.address.Split(',');             
            string[] words4 = op4.address.Split(',');

            if(op_id.Equals("OP1"))
            {
               return words[0];           
            }
            else if (op_id.Equals("OP2"))
            {
                return words2[0];
            }
            else if (op_id.Equals("OP3"))
            {
                return words3[0];
            }
            else if (op_id.Equals("OP4"))
            {
                return words4[0];
            }
            return "Invalid Operator id";
        }

        static void read_command(string command)
        {
            if (command.Equals("start OP1"))
            {
                op_obj1.read_repository(op1.input_ops);
                op_obj1.set_start();             
               
            }
            else if (command.Equals("start OP2"))
            {
                op_obj2.set_start();
            }
            else if (command.Equals("start OP3"))
            {
                op_obj3.set_start();
            }
            else if (command.Equals("start OP4"))
            {
                op_obj4.set_start();
            }
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