using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PUPPET_MASTER
{
     class Puppet_master
    {

        static string semantics;
        static string loggin_level;

        public struct Operator
        {    
            public int operator_id;
            public string input_ops;
            public string rep_factor;
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
          
        }

        static void read_conf_file()
        {
            op1 = new Operator();
            op2 = new Operator();
            op3 = new Operator();
            op4 = new Operator();

            string line;
          
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\lj0se\Desktop\IST\1º Semestre\DAD\DADSTORM\conf_file.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(' ');
                foreach(string s in words)
                {
                    if(words[0] == "Semantics")
                    {
                        semantics = words[1];
                    }
                    if (words[0] == "LoggingLevel")
                    {
                        loggin_level = words[1];
                    }
                    if (words[0] == "OP1")
                    {
                        op1.operator_id = 1;
                        op1.input_ops = words[2];
                        op1.rep_factor = words[4];
                        op1.routing = words[6];
                        op1.address = words[8] + words[9];
                        op1.operator_spec = words[11] + words[12];
                    }
                    if (words[0] == "OP2")
                    {
                        op2.operator_id = 2;
                        op2.input_ops = words[2];
                        op2.rep_factor = words[4];
                        op2.routing = words[6];
                        op2.address = words[8];
                        op2.operator_spec = words[10];
                    }
                    if (words[0] == "OP3")
                    {
                        op3.operator_id = 3;
                        op3.input_ops = words[2];
                        op3.rep_factor = words[4];
                        op3.routing = words[6];
                        op3.address = words[8];
                        op3.operator_spec = words[10];
                    }
                    if (words[0] == "OP4")
                    {
                        op4.operator_id = 4;
                        op4.input_ops = words[2];
                        op4.rep_factor = words[4];
                        op4.routing = words[6];
                        op4.address = words[8];
                        op4.operator_spec = words[10];
                    }
                    
                }
            }
            Console.WriteLine("ALL VARS:" + op1.operator_id +"-"+ op1.input_ops+ "-" +op1.operator_spec+ "-"+ op1.rep_factor+ "-"+ op1.address +"-"+op1.routing);
        }

       static public void print_var()
        {
           

        }
        
    }
}
