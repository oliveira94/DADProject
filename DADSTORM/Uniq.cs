using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace DADSTORM
{
    class Uniq
    {
        static void Main(string[] args)
        {

        }

        static bool is_uniq(List<string> tup, int field_number)
        {
            string str = tup[field_number];
            bool is_uniq = true;
            int counter = 0;

           foreach(string field in tup)
           {
                if(field == str)
                {
                    counter++;
                }
           }

            if(counter > 1)
            {
                is_uniq = false;
            }
            return is_uniq;
        }
    }
}
