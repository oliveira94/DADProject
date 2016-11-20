﻿using System;
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
        void read_repository(string path);
        void input_queue(List<string> tuple);
        void start_processing();
        void set_start();
    }
    public interface IfileToOperator
    {
        IList<IList<string>> CustomOperation(IList<string> l);
    }
    //needs an interface to allow operators communicate with pcs!!!!
}
