using remoting_interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace remoting_interfaces
{
    [Serializable]
    public class Tuple
    {
        int id;
        string user, URL;

        public Tuple(int id, string user, string URL)
        {
            this.id = id;
            this.user = user;
            this.URL = URL;
        }
        public int getID()
        {
            return id;
        }

        public void setID(int id)
        {
            this.id = id;
        }

        public string getUser()
        {
            return user;
        }

        public void setUser(string user)
        {
            this.user = user;
        }

        public string getURL()
        {
            return URL;
        }

        public void setURL(string URL)
        {
            this.URL = URL;
        }
    }

    public interface Ipcs
    {
        void create_replica(int rep_factor, string replica_URL, string whatoperator, string op_id);
    }
    public interface Ipuppet_master
    {

    }
    public interface IOperator
    {
        void next_op(string url, string routing);
        void readFile();
        void set_start(string op_spec, int first);
        void add_to_inQueue(remoting_interfaces.Tuple tp);
        void set_freeze();
        void set_unfreeze();
        void crash();
    }
    public interface IfileToOperator
    {
        IList<IList<string>> CustomOperation(IList<string> l);
    }
}
