using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading.Tasks;
using remoting_interfaces;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;

namespace @operator
{
    //Use the abstract modifier in a class declaration to indicate that a class is intended 
    //only to be a base class of other classes.
    abstract class operators
    {
        //Maybe will be need a struct with 2 parameters, one is the URL, and another is the position but position can be ambiguous
        List<String> tuplos = new List<String>();
        static opObject operatorObject;
        string arg = "";

        public string pathDir = "";

        static void Main(string[] args)
        {
            Console.WriteLine(args[2] + " com url: " + args[0] + " criado com sucesso");

            string[] words = args[0].Split(':'); //split url in order to get to port
            string[] words2 = words[2].Split('/');
            int port = Int32.Parse(words2[0]);

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            operatorObject = new opObject();
            RemotingServices.Marshal(operatorObject, "op", typeof(opObject));
            Console.ReadLine();
        }
    }

    //Receive tweeters.dat
    //This file contains the URLs sent by tweeters. 
    //The format of each line is: id, tweeter id, url

    //Get the people who tweeted about some URL. This is done by reading the ﬁle tweeters.dat containing 
    //tuples [id, tweeter, url] and by emitting, for each tuple in input, tuples of the format [tweeter] if url 
    //is equal to “www.tecnico.ulisboa.pt” 

    public class opObject : MarshalByRefObject, IOperator
    {
        IOperator op_obj;
        static string next_url; //url do proximo operador
        static string next_op_spec; //tipo do proximo operador
        static public bool start = false;
        List<Tuple> queue = new List<Tuple>();

        List<Tuple> in_queue = new List<Tuple>();
        List<Tuple> out_queue = new List<Tuple>();

        static string op_spec;

        IOperator obj; //objecto remoto no proximo operador

        struct Tuple
        {
            public int id;
            public string user;
            public string URL;
        }

        public void next_op(string url, string op_spec)
        {
            next_url = url;
            next_op_spec = op_spec;
            Console.WriteLine("Next URL->" + next_url + "   Next op_spec->" + next_op_spec);

            if (!next_url.Equals("nulo"))
            {
                obj = (IOperator)Activator.GetObject(typeof(IOperator), next_url); // cria um objeto remoto no proximo operador (caso existir)   
            }


        }

        public void set_start(string op_spec_in)
        {
            start = true;

            op_spec = op_spec_in;

            Thread inThread = new Thread(process_inQueue);
            inThread.Start();

            Thread outThread = new Thread(process_outQueue);
            outThread.Start();
        }

        public void process_inQueue()
        {
            Tuple tuple1 = new Tuple();
            tuple1.id = 1;
            tuple1.user = "user2";
            tuple1.URL = "www.ulisboa.pt";

            Tuple tuple2 = new Tuple();
            tuple2.id = 1;
            tuple2.user = "user3";
            tuple2.URL = "www.tecnico.ulisboa.pt";

            Tuple tuple3 = new Tuple();
            tuple3.id = 3;
            tuple3.user = "user3";
            tuple3.URL = "www.tecnico.ulisboa.pt";

            in_queue.Add(tuple1);
            in_queue.Add(tuple2);
            in_queue.Add(tuple3);

            UNIQ uniq = new UNIQ();

            while (true)
            {
                if (in_queue.Count > 0)
                {
                    string[] words = op_spec.Split(',');

                    Tuple outTuple = new Tuple();

                    Console.WriteLine("ID: " + in_queue[0].id);
                    Console.WriteLine("User: " + in_queue[0].user);
                    Console.WriteLine("URL: " + in_queue[0].URL);

                    if(words[0] == "FILTER")
                    {
                        FILTER filter = new FILTER();
                        outTuple = filter.doTweeters(in_queue[0], Int32.Parse(words[1]), words[2], words[3]);
                        in_queue.Remove(in_queue[0]);
                        Console.WriteLine("Output from Operator:");
                        Console.WriteLine(outTuple.id);
                        Console.WriteLine(outTuple.user);
                        Console.WriteLine(outTuple.URL);
                    }
                    if(words[0] == "CUSTOM")
                    {
                        CUSTOM custom = new CUSTOM();
                        custom.getoutput(words[1], words[3], in_queue[0]);
                        in_queue.Remove(in_queue[0]);
                    }
                
                    if(words[0] == "UNIQ")
                    {
                        
                        outTuple = uniq.uniqTuple(in_queue[0], Int32.Parse(words[1]));
                        Console.WriteLine("Output from Operator:");
                        Console.WriteLine(outTuple.id);
                        Console.WriteLine(outTuple.user);
                        Console.WriteLine(outTuple.URL);
                        in_queue.Remove(in_queue[0]);
                    }   
                }
            }
        }

        public void process_outQueue()
        {
            while (true)
            {
                if (!next_url.Equals("nulo"))
                {
                    //obj.input_queue(out_queue[0]);                          
                }
            }
        }

        //public void input_queue(Tuple tuple)
        //{
        //    if (!start || queue.Count > 0)                 // se existir uma fila de espera ou o estado do operador for inativo
        //    {
        //        queue.Add(tuple);                          //coloca o tuplo na ultima posição da fila de espera
        //    }
        //    else
        //    {
        //        teste_processa(tuple);                     //caso não exista uma fila e o estado for ativo o tuplo é processado; O processamento real seria buscar o valor de args[1] para ver qual é este operador, e enviar para a função de processamento correspondente (DUP, FILTER...) 
        //    }
        //}

        public void teste_processa(List<string> tp) // imprime o tuplo e envia para a queue do proximo operador(caso exista)
        {
            Console.Write("Input Tuplo: ");
            foreach (string s in tp)
            {
                Console.Write(s + " ");
            }

            Console.WriteLine("\r\n");

            if (!next_url.Equals("nulo"))  //se houver proximo operador
            {
                //obj.input_queue(tp);   // tem que ser assincrono para não ficar à espera do op2                             
            }
        }

        //when the input of a operator is a directory/file
        public List<String> read_repository(string path, string op_spec)
        {

            string[] words = op_spec.Split(',');

            string final_path = Directory.GetCurrentDirectory();
            final_path.Remove(final_path.Length - 40);
            final_path = final_path + path;
            Console.WriteLine("Repository at: " + path);

            Tuple tuple = new Tuple();

            List<string> tup_test = new List<string>(); // vai criar um tuplo de teste e colocar na sua queue (esta é uma função do OP1)

            if (words[0] == "FILTER")
            {
                FILTER filter = new FILTER();
                //tuple = filter.doTweeters( path, Int32.Parse(words[1]), words[2], words[3]);
                //List<List<String>> teste = filter.getTweeters(words[3], path, words[2], Int32.Parse(words[1]));

                //foreach (List<string> subList in teste)
                //{
                //    foreach (string item in subList)
                //    {
                //        tup_test.Add(item);
                //    }
                //}
            }
            //input_queue(tup_test);
            return tup_test;
        }

        //when the input of a operator is a list of strings
        public List<string> read_listOfStrings(List<string> input, string op_spec)
        {
            string[] words = op_spec.Split(',');
            List<string> receiveoutput = new List<string>();

            if (words[0] == "CUSTOM")
            {
                CUSTOM custom = new CUSTOM();
                foreach (string user in input)
                {
                    //receiveoutput = custom.getoutput(words[1], words[3], user);
                }
            }
            if (words[0] == "UNIQ")
            {

            }
            if (words[0] == "DUP")
            {

            }

            //foreach (string list in input)
            //{
            //    Console.WriteLine(list);
            //}

            //input_queue(input);
            return receiveoutput;
        }

        class FILTER : operators
        {
            public Tuple doTweeters(Tuple input_tuple, int field_number, string condition, string value)
            {
                List<string> tweeters = new List<string>();

                Tuple tuple = new Tuple();

                string[] tokens = { input_tuple.id.ToString(), input_tuple.user, input_tuple.URL };

                        //know what field_number(one, two or three)
                        //field_number is ID
                        if(field_number == 1)
                        {
                            switch (condition)
                            {
                                case "<":
                                    if(Int32.Parse(tokens[0]) < Int32.Parse(value))
                                    {
                                        tuple.id = Int32.Parse(tokens[0]);
                                        tuple.user = tokens[1];
                                        tuple.URL = tokens[2];
                                    }
                                    break;
                                case ">":
                                    if (Int32.Parse(tokens[0]) > Int32.Parse(value))
                                    {
                                        tuple.id = Int32.Parse(tokens[0]);
                                        tuple.user = tokens[1];
                                        tuple.URL = tokens[2];
                                    }
                                    break;
                                case "=":
                                    if (Int32.Parse(tokens[0]) == Int32.Parse(value))
                                    {
                                
                                        tuple.id = Int32.Parse(tokens[0]);
                                        tuple.user = tokens[1];
                                        tuple.URL = tokens[2];
                                    }
                                    break;
                                default:
                                    Console.WriteLine("default");
                                    break;
                            }
                        }
                        //field_number is users
                        else if(field_number == 2)
                        {
                           if(value.Contains(tokens[1]))
                            {
                                tuple.id = Int32.Parse(tokens[0]);
                                tuple.user = tokens[1];
                                tuple.URL = tokens[2];
                            }
                        }
                        //field_nember is the URLs
                        else if (field_number == 3)
                        {
                            if (value.Contains(tokens[2]))
                            {
                                tuple.id = Int32.Parse(tokens[0]);
                                tuple.user = tokens[1];
                                tuple.URL = tokens[2];
                            }
                        }
                return tuple;
            }
        }

        // where enter the followers.dat and the mylib.dll wheer
        class CUSTOM : operators
        {
            public List<String> getoutput(string dll, string method, Tuple tuple)
            {
                List<string> outputUsers = new List<string>();
                Assembly testDLL = Assembly.LoadFile(@"\\Mac\Home\Documents\GitHub\FindSomethingElse\DADSTORM\" + dll);

                foreach (var type in testDLL.GetExportedTypes())
                {
                    MethodInfo[] members = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

                    foreach (MemberInfo member in members)
                    {
                        if (member.Name.Equals(method))
                        {
                            MethodInfo methodInfo = type.GetMethod(member.Name);

                            if (methodInfo != null)
                            {
                                IList<IList<string>> result;
                                ParameterInfo[] parameters = methodInfo.GetParameters();
                               
                                object classInstance = Activator.CreateInstance(type, null);

                                if (parameters.Length == 0)
                                {
                                    result = (IList<IList<string>>)methodInfo.Invoke(classInstance, null);
                                }
                                else
                                {
                                    object[] arr4 = new object[1];

                                    List<string> inputLista = new List<string>();
                                    inputLista.Add(tuple.id.ToString());
                                    inputLista.Add(tuple.user);
                                    

                                    arr4[0] = inputLista;

                                    result = (IList<IList<string>>)methodInfo.Invoke(classInstance, arr4);
                                    Console.WriteLine(tuple.user);
                                    foreach (List<string> outputlist in result)
                                    {
                                        foreach (string output in outputlist)
                                        {
                                            outputUsers.Add(output);
                                            Console.WriteLine(output);

                                        }

                                    }
                                }
                            }
                        }

                    }
                }
                return outputUsers;
            }
        }

        class UNIQ : operators
        {
            List<string> tuplos = new List<string>();
            Tuple output = new Tuple();

            public Tuple uniqTuple(Tuple tuple, int field_nember)
            {
                if(field_nember == 1)
                {
                    if (!tuplos.Contains(tuple.id.ToString()))
                    {
                        tuplos.Add(tuple.id.ToString());
                        output = tuple;
                    }
                    else
                    {
                        output = new Tuple();
                    }
                }
                else if(field_nember == 2)
                {
                    if (!tuplos.Contains(tuple.user))
                    {
                        tuplos.Add(tuple.user);
                        output = tuple;
                    }
                    else
                    {
                        output = new Tuple();
                    }
                }
                else if (field_nember == 3)
                {
                    if (!tuplos.Contains(tuple.URL))
                    {
                        tuplos.Add(tuple.URL);
                        output = tuple;
                    }
                    else
                    {
                        output = new Tuple();
                    }
                }

                return output;
            }
        }

        class DUP : operators
        {
            List<String> returnInput(List<String> tuple)
            {
                return tuple;
            }
        }

        class ThreadManager
        {

        }
    }

}

