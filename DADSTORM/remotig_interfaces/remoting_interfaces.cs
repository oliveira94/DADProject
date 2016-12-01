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
        void next_op(string url);
        void readFile();
        void set_start(string op_spec);
    }
    public interface IfileToOperator
    {
        IList<IList<string>> CustomOperation(IList<string> l);
    }
}
