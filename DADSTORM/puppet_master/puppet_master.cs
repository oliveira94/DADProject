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
        static IOperator op_obj;
  
        public struct Operator
        {
            public string operator_id;
            public string input_ops;
            public int rep_factor;
            public string routing;
            public string address;
            public string operator_spec;
        }

        static List<Operator> op_list = new List<Operator>(); //lista de todos os operadores
        static List<IOperator> op_obj_list = new List<IOperator>();//lista de todos os objetos que existem nos operadores

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
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"..\..\..\conf_file.txt");

            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(' ');
                foreach (string word in words)
                {
                    if (word == "Semantics")
                    {
                        semantics = words[1];
                    }
                    else if (word == "LoggingLevel")
                    {
                        loggin_level = words[1];
                    }
                    else if (word.StartsWith("OP") && word == words[0]) // se a palavra for começar por OP e for a primeira palavra da lista de palavras
                    {
                        Operator op = new Operator();
                        op.operator_id = words[0];
                        op.input_ops = words[2];
                        op.rep_factor = Int32.Parse(words[4]);
                        op.routing = words[6];
                        op.address = words[8];

                        for (int i = 1; i < op.rep_factor; i++) //para o apanhar o numero de URLs especificado em rep_factor 
                        {
                            op.address = op.address + words[8 + i];
                        }

                        op.operator_spec = words[8 + op.rep_factor + 1]; // guardamos o tipo de operador 
                        if (!op.operator_spec.Equals("COUNT")) // Se o tipo for diferente de "count" significa que ainda falta concatenar os parametros
                        {
                            op.operator_spec = op.operator_spec + "," + words[(8 + op.rep_factor + 1) + 1]; //concatenamos o tipo de operador com os seus parametros
                        }
                        op_list.Add(op);
                    }
                }
            }
        }

        static public void create_replicas()
        {
            pcs_obj = (Ipcs)Activator.GetObject(typeof(Ipcs), "tcp://localhost:10000/pcs");

            funcaoCallBack = new AsyncCallback(OnExit);//aponta para a função de retorno da função assincrona
            RemoteAsyncDelegate dele = new RemoteAsyncDelegate(pcs_obj.create_replica);//aponta para a função a ser chamada assincronamente

            foreach (Operator op in op_list)
            {
                IAsyncResult result = dele.BeginInvoke(op.rep_factor, op.address, op.operator_spec, op.operator_id, funcaoCallBack, null);

            }

        }

        static public void next_Operator() //envia informação a cada operador sobre o operador seguinte
        {

            for (int i = 0; i < (op_list.Count - 1); i++)
            {
                string[] words = op_list[i].address.Split(','); //cria uma lista com os URLs de todos do operador atual
                foreach (string url in words)
                {
                    op_obj = (IOperator)Activator.GetObject(typeof(IOperator), url); //cria um objeto remoto na replica do operador atual
                    op_obj.next_op(op_list[i + 1].address, op_list[i + 1].routing); //envia ao operador os URLs do operador downstream
                }
            }

        }
        private static string routing(string urls, string routing)
        {
            string[] words = urls.Split(',');       // contem todos os URls do  downstream operador 
            if (routing.Equals("primary"))
            {
                return words[0];
            }
            else if (routing.Equals("random"))
            {
                Random rnd = new Random();
                int rep = rnd.Next((words.Length)); // gera um numero de 1 até ao replication factor do operador
                return words[rep];
            }
            else
            {
                return words[0];
            }
            return "error";
        }

        static void read_command(string command)
        {
            List<string> output = new List<string>();
            string[] words = command.Split(' ');
            try
            {
                if (command.StartsWith("start"))
                {
                    foreach (Operator op in op_list)
                    {
                        if (op.operator_id.Equals(words[1]))
                        {
                            if (words[1].Equals("OP1"))
                            {
                                op_obj = (IOperator)Activator.GetObject(typeof(IOperator), routing(op.address, op.routing));
                                op_obj.set_start(op.operator_spec, 0);
                            }
                            else
                            {
                                string[] rep = op.address.Split(',');

                                foreach (string url in rep)
                                {
                                    op_obj = (IOperator)Activator.GetObject(typeof(IOperator), url);
                                    op_obj.set_start(op.operator_spec,1);
                                }
                            }
                        }                     
                    }
                }
                else if (command.Equals("freeze OP1"))
                {

                }
                else if (command.Equals("unfreeze OP1"))
                {

                }
                else if (command.Equals("crash OP1"))
                {

                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                Console.WriteLine("Operator crashed.");
            }
        }

        static public void print_var() // testar se as variáveis foram guardadas de acordo com o ficheiro de configuração
        {
            Console.WriteLine("ALL vars: \r\n");
            foreach (Operator op in op_list)
            {
                Console.WriteLine("-operator_id " + op.operator_id + " -input:" + op.input_ops + " -rep_fact:" + op.rep_factor + " -routing:" + op.routing + " -address:" + op.address + " -operator:" + op.operator_spec + "\r\n");
            }
        }

        public static void OnExit(IAsyncResult ar)
        {
             
        }
    }

    public class puppet_master_object : MarshalByRefObject, Ipuppet_master
    {

    }
}