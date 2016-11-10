using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace INTERFACES
{
    public interface Ipcs
    {
         void create_replica(int rep_factor, string replica_URL, string WhatOperator);
    }
    public interface Ipuppet_master
    {

    }
}
