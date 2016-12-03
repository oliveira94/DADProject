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

   
    

public class opObject : MarshalByRefObject, IOperator
    {
        IOperator op_obj;
        static string next_url; //url do proximo operador
        static string next_op_spec; //tipo do proximo operador
        static public bool start = false;
        List<remoting_interfaces.Tuple> queue = new List<remoting_interfaces.Tuple>();

        List<remoting_interfaces.Tuple> in_queue = new List<remoting_interfaces.Tuple>();
        List<remoting_interfaces.Tuple> out_queue = new List<remoting_interfaces.Tuple>();

        static string op_spec;

        IOperator obj; //objecto remoto no proximo operador

        //struct remoting_interfaces.Tuple
        //{
        //    public int id;
        //    public string user;
        //    public string URL;
        //}

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

        public void set_start(string op_spec_in, int firstTime)
        {
            start = true;
            op_spec = op_spec_in;

            if(firstTime == 0)
            {
                //thread to convert each line of tweeters.dat in a remoting_interfaces.Tuple
                Thread readData = new Thread(readFile);
                readData.Start();
            }
            
            Thread inThread = new Thread(process_inQueue);
            inThread.Start();

            Thread outThread = new Thread(process_outQueue);
            outThread.Start();
        }

        //Converto each userInfo in a remoting_interfaces.Tuple
        public void readFile()
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(@"..\..\..\tweeters.data");
            
            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                remoting_interfaces.Tuple Tuple = new remoting_interfaces.Tuple(Int32.Parse(words[0]), words[1], words[2]);
                in_queue.Add(Tuple);
            }
        }

        public void process_inQueue()
        {
            remoting_interfaces.Tuple Tuple1 = new remoting_interfaces.Tuple(1, "user2", "www.ulisboa.pt");
            remoting_interfaces.Tuple Tuple2 = new remoting_interfaces.Tuple(1, "user3", "www.tecnico.ulisboa.pt");
            remoting_interfaces.Tuple Tuple3 = new remoting_interfaces.Tuple(3, "user3", "www.tecnico.ulisboa.pt");

            in_queue.Add(Tuple1);
            in_queue.Add(Tuple2);
            in_queue.Add(Tuple3);

            FILTER filter = new FILTER();
            CUSTOM custom = new CUSTOM();
            UNIQ uniq = new UNIQ();
            DUP dup = new DUP();
            COUNT count = new COUNT();

            while (true)
            {
                Thread.Sleep(2000);

                if (in_queue.Count > 0)
                {
                    string[] words = op_spec.Split(',');



                    remoting_interfaces.Tuple outTuple;
                    
                    Console.WriteLine("   ");
                    Console.WriteLine("ID: " + in_queue[0].getID());
                    Console.WriteLine("User: " + in_queue[0].getUser());
                    Console.WriteLine("URL: " + in_queue[0].getURL());

                    if(words[0] == "FILTER")
                    {
                        outTuple = filter.doTweeters(in_queue[0], Int32.Parse(words[1]), words[2], words[3]);
                        out_queue.Add(outTuple);
                        in_queue.Remove(in_queue[0]);
                        Console.WriteLine("Output from Operator:");
                        Console.WriteLine(outTuple.getID());
                        Console.WriteLine(outTuple.getUser());
                        Console.WriteLine(outTuple.getURL());
                    }
                    if(words[0] == "CUSTOM")
                    {
                        List<string> Followers = new List<string>();

                        Console.WriteLine(in_queue[0]);

                        Followers = custom.getoutput(words[1], words[3], in_queue[0]);
                        foreach(string follower in Followers)
                        {
                            Console.WriteLine("follower: " + follower);
                            remoting_interfaces.Tuple Tuple = new remoting_interfaces.Tuple(0, follower,"");
                            out_queue.Add(Tuple);
                        }
                        
                        in_queue.Remove(in_queue[0]);
                    }
                    if(words[0] == "UNIQ")
                    {
                        outTuple = uniq.uniqTuple(in_queue[0], Int32.Parse(words[1]));
                        out_queue.Add(outTuple);
                        Console.WriteLine("Output from Operator:");
                        Console.WriteLine(outTuple.getID());
                        Console.WriteLine(outTuple.getUser());
                        Console.WriteLine(outTuple.getURL());
                        in_queue.Remove(in_queue[0]);
                    }   
                    if(words[0] == "DUP")
                    {
                        List<remoting_interfaces.Tuple> duplicatedTuple = dup.duplicate(in_queue[0]);
                        
                        foreach (remoting_interfaces.Tuple tuplo in duplicatedTuple)
                        {
                            out_queue.Add(tuplo);
                            Console.WriteLine("Output from Operator:");
                            Console.WriteLine(tuplo.getID());
                            Console.WriteLine(tuplo.getUser());
                            Console.WriteLine(tuplo.getURL());
                        }
                        duplicatedTuple.Remove(in_queue[0]);
                        duplicatedTuple.Remove(in_queue[0]);

                        in_queue.Remove(in_queue[0]);
                    }
                    if(words[0] == "COUNT")
                    {
                        outTuple = count.countMethod(in_queue[0]);
                        out_queue.Add(outTuple);
                        Console.WriteLine("Output from Operator:");
                        Console.WriteLine(outTuple.getID());
                        Console.WriteLine(outTuple.getUser());
                        Console.WriteLine(outTuple.getURL());
                        Console.WriteLine("Tuples count until now: " + count.getCount());
                        in_queue.Remove(in_queue[0]);
                    }
                }
            }
        }

        public void process_outQueue()
        {
            while (true)
            {
                if (!next_url.Equals("nulo") )
                {
                    if(out_queue.Count > 0)
                    {
                        //obj.input_queue(out_queue[0]);
                        //out_queue.Remove(out_queue[0]);
                    }
                                            
                }
            }
        }

        public void input_queue(remoting_interfaces.Tuple tuple)
        {
            if (!start || queue.Count > 0)                 //se existir uma fila de espera ou o estado do operador for inativo
            {
                queue.Add(tuple);                          //coloca o tuplo na ultima posição da fila de espera
            }
        }

        class FILTER : operators
        {
            public remoting_interfaces.Tuple doTweeters(remoting_interfaces.Tuple input_Tuple, int field_number, string condition, string value)
            {
                List<string> tweeters = new List<string>();

                remoting_interfaces.Tuple Tuple = new remoting_interfaces.Tuple(0, "", "");

                string[] tokens = { input_Tuple.getID().ToString(), input_Tuple.getUser(), input_Tuple.getURL() };

                        //know what field_number(one, two or three)
                        //field_number is ID
                        if(field_number == 1)
                        {
                            switch (condition)
                            {
                                case "<":
                                    if(Int32.Parse(tokens[0]) < Int32.Parse(value))
                                    {
                                        Tuple.setID(Int32.Parse(tokens[0]));
                                        Tuple.setUser(tokens[1]);
                                        Tuple.setURL(tokens[2]);
                                    }
                                    break;
                                case ">":
                                    if (Int32.Parse(tokens[0]) > Int32.Parse(value))
                                    {
                                        Tuple.setID(Int32.Parse(tokens[0]));
                                        Tuple.setUser(tokens[1]);
                                        Tuple.setURL(tokens[2]);
                            }
                                    break;
                                case "=":
                                    if (Int32.Parse(tokens[0]) == Int32.Parse(value))
                                    {
                                        Tuple.setID(Int32.Parse(tokens[0]));
                                        Tuple.setUser(tokens[1]);
                                        Tuple.setURL(tokens[2]);
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
                           if(tokens[1].Contains(value))
                            {
                                Tuple.setID(Int32.Parse(tokens[0]));
                                Tuple.setUser(tokens[1]);
                                Tuple.setURL(tokens[2]);
                    }
                        }
                        //field_nember is the URLs
                        else if (field_number == 3)
                        {
                 
                            if (tokens[2].Contains(value))
                            {
                                Tuple.setID(Int32.Parse(tokens[0]));
                                Tuple.setUser(tokens[1]);
                                Tuple.setURL(tokens[2]);
                    }
                        }
                return Tuple;
            }
        }

        // where enter the followers.dat and the mylib.dll wheer
        class CUSTOM : operators
        {
            public List<String> getoutput(string dll, string method, remoting_interfaces.Tuple Tuple)
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
                                    inputLista.Add(Tuple.getID().ToString());
                                    string str = Tuple.getUser();
                                    str = str.Replace(" ", String.Empty);
                                    inputLista.Add(str);
                                    arr4[0] = inputLista;

                                    result = (IList<IList<string>>)methodInfo.Invoke(classInstance, arr4);
                                    
                                    foreach (List<string> outputlist in result)
                                    {
                                        foreach (string output in outputlist)
                                        {
                                            outputUsers.Add(output);
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
            remoting_interfaces.Tuple output = new remoting_interfaces.Tuple(0, "", "");

            public remoting_interfaces.Tuple uniqTuple(remoting_interfaces.Tuple Tuple, int field_nember)
            {
                if(field_nember == 1)
                {
                    if (!tuplos.Contains(Tuple.getID().ToString()))
                    {
                        tuplos.Add(Tuple.getID().ToString());
                        output = Tuple;
                    }
                    else
                    {
                        output = new remoting_interfaces.Tuple(0, "", "");
                    }
                }
                else if(field_nember == 2)
                {
                    if (!tuplos.Contains(Tuple.getUser()))
                    {
                        tuplos.Add(Tuple.getUser());
                        output = Tuple;
                    }
                    else
                    {
                        output = new remoting_interfaces.Tuple(0, "", "");
                    }
                }
                else if (field_nember == 3)
                {
                    if (!tuplos.Contains(Tuple.getURL()))
                    {
                        tuplos.Add(Tuple.getURL());
                        output = Tuple;
                    }
                    else
                    {
                        output = new remoting_interfaces.Tuple(0, "", "");
                    }
                }

                return output;
            }
        }

        class DUP : operators
        {
            List<remoting_interfaces.Tuple> listToDup = new List<remoting_interfaces.Tuple>();

            public List<remoting_interfaces.Tuple> duplicate(remoting_interfaces.Tuple Tuple)
            {
                listToDup.Add(Tuple);
                listToDup.Add(Tuple);

                return listToDup;
            }
        }

        class COUNT : operators
        {
            int count = 0;

            public remoting_interfaces.Tuple countMethod(remoting_interfaces.Tuple Tuple)
            {
                count++;
                return Tuple;
            }

            public int getCount()
            {
                return count;
            }
        }

        class ThreadManager
        {

        }
    }
}

