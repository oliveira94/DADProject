using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace remoting_interfaces
{
    public interface Ipcs
    {
        void create_replica(int rep_factor, string replica_URL, string whatoperator, string op_id);
    }
    public interface Ipuppet_master
    {

    }
    public interface IOperator
    {
        void next_op(string url, string op_spec);
        void input_queue(List<string> tuple);
        void set_start();
        List<string> read_repository(string input_ops, string operator_spec);
        List<string> read_listOfStrings(List<string> input, string op_spec);
    }
    public interface IfileToOperator
    {
        IList<IList<string>> CustomOperation(IList<string> l);
    }
    //needs an interface to allow operators communicate with pcs!!!!
}
