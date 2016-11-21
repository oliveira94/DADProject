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



namespace @operator
{
    //Use the abstract modifier in a class declaration to indicate that a class is intended 
    //only to be a base class of other classes.
    abstract class operators
    {
        //Maybe will be need a struct with 2 parameters, one is the URL, and another is the position but position can be ambiguous
        List<String> tuplos = new List<String>();
        static opObject operatorObject;

        static void Main(string[] args)
        {
            Console.WriteLine(args[2] + " com url: "+ args[0] + " criado com sucesso");
           
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
        List<List<string>> queue = new List<List<string>>(); 
        IOperator obj; //objecto remoto no proximo operador

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

        public void set_start()
        {
            int i = 0;
            start = true;
            while(queue.Count > 0)
            {
                teste_processa(queue[i]);                   // processa o primeiro elemento da fila
                queue.RemoveAt(i);                          // remove-o da fila de espera
            }
                 
        }
        public void input_queue(List<string> tuple)
        {
            if (!start || queue.Count > 0)                 // se existir uma fila de espera ou o estado do operador for inativo
            {
                queue.Add(tuple);                          //coloca o tuplo na ultima posição da fila de espera
            }
            else
            {
                teste_processa(tuple);                     //caso não exista uma fila e o estado for ativo o tuplo é processado
            }

        }
        
      

    
        public void teste_processa(List<string> tp) // imprime o tuplo e envia para a queue do proximo operador(caso exista)
        {
            Console.Write("Tuplo-");
            foreach(string s in tp)
            {
                Console.Write(s+",");
            }
            Console.WriteLine("\r\n");

            if (!next_url.Equals("nulo"))  //se houver proximo operador
            {
                obj.input_queue(tp);   // tem que ser assincrono para não ficar à espera do op2                             
            }        
        }

        public void read_repository(string path)
        {
            
            string final_path = Directory.GetCurrentDirectory();
            final_path.Remove(final_path.Length - 18);
            final_path = final_path + path;
            Console.WriteLine("Repository at: " + path);
            
            List<string> tup_test = new List<string>(); // vai criar um tuplo de teste e colocar na sua queue (esta é uma função do OP1)
            tup_test.Add("Este");
            tup_test.Add("é");
            tup_test.Add("um");
            tup_test.Add("teste");
            input_queue(tup_test);
        }
       

        class FILTER : operators
        {

            int field_number, condition, value;

            //ﬁeld number, condition, value: emit the input tuple if ﬁeld number is larger(”>”), smaller(”<”) or equal(”=”) than value. 
            public FILTER(int field_number, int condition, int value)
            {
                this.field_number = field_number;
                this.condition = condition;
                this.value = value;
            }

            //after input the field number, the condition and the value, we will get a subset of tuples
            // i'm assuming that the tuples are ints just to simplify a possible implementation
            /*  int analyse(List<int> returnedTuplos)
              {
                  for (int i = 0; i < returnedTuplos.Count; i++)
                  {
                      if (returnedTuplos[i] < value)
                          condition = -1;
                      if (returnedTuplos[i] > value)
                          condition = 1;

                      switch (condition)
                      {
                          //when condition is <
                          case -1:
                              return returnedTuplos[i];
                              break;
                          //when conditiob is >
                          case 1:
                              return returnedTuplos[i];
                              break;
                          //when condition is =
                          default:
                              return returnedTuplos[i];
                              break;
                      }
                  }
              }
         */   
       }

            // where enter the followers.dat and the mylib.dll wheer
            class CUSTOM : operators
            {
              
            }


            class UNIQ : operators
            {           
                int field_number;

                public UNIQ(int field_number)
                {
                    this.field_number = field_number;
                }

                // i'm assuming that the tuples are ints just to simplify a possible implementation
                /*  int processTuple(List<int> tuple)
                  {
                      int returnValue = -1;
                      for(int i = 0; i < tuple.Count; i++)
                      {
                          if (tuple[i] == field_number)
                          {
                              returnValue = tuple[i];
                              return returnValue;
                          }    
                      }
                  }
                 */
            }
            class DUP : operators
            {
                List<String> returnInput(List<String> tuple)
                {
                    return tuple;
                }
            }
    }

 }

