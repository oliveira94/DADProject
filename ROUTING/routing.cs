using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ROUTING
{
    class routing
    {
        
        static void Main(string[] args)
        {
            primary();
        }

        static int rep_factor() 
        {
            return 3;
        }

        static private void primary()
        {
            ThreadStart ts = new ThreadStart(routing.replica1);
            Thread t = new Thread(ts);//threads means sending the tuple
            t.Start(); // here we send the tuple to the URL of the 1st replica 
        }
        static private void random()
        {
            int alive = rep_factor();
            Random random = new Random();
            int rand = random.Next(1, alive); //we send the tuple to the random replica URL


        }
        static private void hashing(int field_id)
        {

        }

        static private void replica1()
        {
            Console.WriteLine("Routed to the replica 1");
        }
       static private  void replica2()
        {
            Console.WriteLine("Routed to the replica 2");
        }
      static private void replica3()
        {
            Console.WriteLine("Routed to the replica 3");
        }
    }
}
