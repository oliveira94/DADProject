using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @operator
{
    //Use the abstract modifier in a class declaration to indicate that a class is intended 
    //only to be a base class of other classes.
    abstract class operators
    {
        //Maybe will be need a struct with 2 parameters, one is the URL, and another is the position but position can be ambiguous
        List<String> tuplos = new List<String>();

        static void Main(string[] args)
        {
            Console.WriteLine(args[0] + " criado com sucesso");
            Console.ReadLine();
        }
    }

    class UNIQ : operators
    {
        int field_number;

        public UNIQ(int field_number)
        {
            this.field_number = field_number;
        }

        // i'm assuming that the tuples are ints just to simplify a possible implementation
        int processTuple(List<int> tuple)
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
    }

    class COUNT : operators
    {
        int countInput(List<String> tuple)
        {
            int numberOfTuples = 0;

            for(int i = 1; i < tuple.Count; i++)
            {
                numberOfTuples++;
            }
            return numberOfTuples;
        }
    }

    class DUP : operators
    {
        List<String> returnInput(List<String> tuple)
        {
            return tuple;
        }
    }

    class FILTER : operators
    {
        int condition, value;

        public FILTER( int condition, int value)
        {
            
            this.condition = condition;
            this.value = value;
        }

        //after input the field number, the condition and the value, we will get a subset of tuples
        // i'm assuming that the tuples are ints just to simplify a possible implementation
        int analyse(List<int> returnedTuplos)
        {
            for(int i = 0; i < returnedTuplos.Count; i++)
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
    }

    class CUSTOM : operators
    {

    }

}
