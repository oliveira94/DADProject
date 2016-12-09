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
        Ipuppet_master puppet_obj = (Ipuppet_master)Activator.GetObject(typeof(Ipuppet_master), "tcp://localhost:10001/puppet_master");

        static string next_url = "null"; //url do proximo operador
        static string next_routing = "null"; //tipo de routing do operador downstream
        static public bool start = false;
        static string operator_id;
        static string log_lvl;
        List<remoting_interfaces.Tuple> queue = new List<remoting_interfaces.Tuple>();

        List<remoting_interfaces.Tuple> in_queue = new List<remoting_interfaces.Tuple>();
        List<remoting_interfaces.Tuple> out_queue = new List<remoting_interfaces.Tuple>();

        static string op_spec;

        static object tLock = new object();

        IOperator obj; //objecto remoto no proximo operador

        public void next_op(string url, string route)
        {

            next_url = url;
            next_routing = route;
            Console.WriteLine("Next URL_list->" + next_url);
            Console.WriteLine("Next OP routing->" + next_routing);
        }

        public void set_start(string op_spec_in, int firstTime, string op_id, string logging_level)
        {
            operator_id = op_id;
            log_lvl = logging_level;
            start = true;
            op_spec = op_spec_in;
            Console.WriteLine("Triggered");
            puppet_obj.log(operator_id + " has started.");

            if (firstTime == 0)
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

        public void set_freeze()
        {
            puppet_obj.log("Command on" + operator_id + ": Freeze");
            Monitor.Enter(tLock);
            try
            {
                Console.WriteLine("Operator frozen.");
            }
            finally
            {

            }
        }

        public void set_unfreeze()
        {
            puppet_obj.log("Command" + operator_id + ": Unfreeze");
            Monitor.Exit(tLock);
        }

        public void crash()
        {
            puppet_obj.log("Command" + operator_id + ": Crash");
            Environment.Exit(0);
        }

        public void Wait(int time)
        {
            puppet_obj.log("Command" + operator_id + ": Interval");
            Monitor.Enter(tLock);
            try
            {
                Thread.Sleep(time);
            }
            finally
            {
                Monitor.Exit(tLock);
            }
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
        public void add_to_inQueue(remoting_interfaces.Tuple tp)
        {
            in_queue.Add(tp);
        }
        public void process_inQueue()
        {
            remoting_interfaces.Tuple Tuple1 = new remoting_interfaces.Tuple(1, "user2", "www.ulisboa.pt");
            remoting_interfaces.Tuple Tuple2 = new remoting_interfaces.Tuple(1, "user3", "www.tecnico.ulisboa.pt");
            remoting_interfaces.Tuple Tuple3 = new remoting_interfaces.Tuple(3, "user3", "www.tecnico.ulisboa.pt");

            //in_queue.Add(Tuple1);
            //in_queue.Add(Tuple2);
            //in_queue.Add(Tuple3);

            FILTER filter = new FILTER();
            CUSTOM custom = new CUSTOM();
            UNIQ uniq = new UNIQ();
            DUP dup = new DUP();
            COUNT count = new COUNT();

            while (true)
            {
                Thread.Sleep(2000);

                Monitor.Enter(tLock);
                try
                {
                    if (in_queue.Count > 0)
                    {
                        string[] words = op_spec.Split(',');

                        remoting_interfaces.Tuple outTuple;

                        if(in_queue[0].getID() != 0)
                        {
                            Console.WriteLine("   ");
                            Console.WriteLine("ID: " + in_queue[0].getID());
                            Console.WriteLine("User: " + in_queue[0].getUser());
                            Console.WriteLine("URL: " + in_queue[0].getURL());
                        }

                        if (words[0] == "FILTER")
                        {
                            outTuple = filter.doTweeters(in_queue[0], Int32.Parse(words[1]), words[2], words[3]);
                            out_queue.Add(outTuple);
                            in_queue.Remove(in_queue[0]);
                            Console.WriteLine("Output from Operator:");
                            Console.WriteLine(outTuple.getID());
                            Console.WriteLine(outTuple.getUser());
                            Console.WriteLine(outTuple.getURL());
                        }
                        if (words[0] == "CUSTOM")
                        {
                            List<string> Followers = new List<string>();

                            Followers = custom.getoutput(words[1], words[3], in_queue[0]);
                            foreach (string follower in Followers)
                            {
                                Console.WriteLine("follower: " + follower);
                                remoting_interfaces.Tuple Tuple = new remoting_interfaces.Tuple(0, follower, "");
                                out_queue.Add(Tuple);
                            }

                            in_queue.Remove(in_queue[0]);
                        }
                        if (words[0] == "UNIQ")
                        {
                            outTuple = uniq.uniqTuple(in_queue[0], Int32.Parse(words[1]));
                            if(outTuple.getUser() != "")
                            {
                                out_queue.Add(outTuple);
                                Console.WriteLine("Output from Operator:");
                                Console.WriteLine(outTuple.getUser());
                            }
                            in_queue.Remove(in_queue[0]);
                        }
                        if (words[0] == "DUP")
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
                        if (words[0] == "COUNT")
                        {
                            outTuple = count.countMethod(in_queue[0]);
                            out_queue.Add(outTuple);
                            
                            Console.WriteLine("Output from Operator:");
                            Console.WriteLine(outTuple.getUser());
                            Console.WriteLine("Tuples count until now: " + count.getCount());
                            Console.WriteLine("      ");
                            
                            in_queue.Remove(in_queue[0]);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(tLock);
                }
            }
        }

        public void process_outQueue()
        {
           while(true)
           {
               if(out_queue.Count > 0)
               {
                   if (!next_url.Equals("null"))
                   {
                        op_obj = (IOperator)Activator.GetObject(typeof(IOperator), routing(next_url, next_routing, out_queue[0]));
                        op_obj.add_to_inQueue(out_queue[0]);
                        if (log_lvl.Equals("full"))
                        {
                            puppet_obj.log("Sent Tuple to " + operator_id);
                        }
                        out_queue.Remove(out_queue[0]);
                   }
               }
           }
        }

        private static string routing(string urls, string routing, remoting_interfaces.Tuple tup)
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
                char[] p = { '(', ')' };
                string[] value = routing.Split(p);
                int rep = hashing(Int32.Parse(value[1]), tup, words.Length);
                return words[rep];

            }
            return "error";
        }

        private static int hashing(int field, remoting_interfaces.Tuple tp, int replicas)
        {
            if (field == 1) // get the tuple id field
            {
                string str = tp.getID().ToString();
                return Math.Abs(str.GetHashCode()) % replicas;
            }
            else if (field == 2) // get the tuple user field
            {
                return Math.Abs(tp.getUser().GetHashCode()) % replicas;
            }
            else if (field == 3) // get the tuple url field
            {
                return Math.Abs(tp.getURL().GetHashCode()) % replicas;
            }
            return 0;
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
                string path = Directory.GetCurrentDirectory();
                path = path.Remove(path.Length - 13);
                Assembly testDLL = Assembly.LoadFile(@path + dll);

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
                    if (!tuplos.Contains(Tuple.getUser()))
                    {
                        tuplos.Add(Tuple.getUser());
                        output = Tuple;
                    }
                    else
                    {
                        output = new remoting_interfaces.Tuple(0, "", "");
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

