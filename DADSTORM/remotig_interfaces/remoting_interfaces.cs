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
        int id, uniqueID;
        string user, URL, timestamp;

        public Tuple(int id, string user, string URL, int uniqueID, string timestamp)
        {
            this.id = id;
            this.user = user;
            this.URL = URL;
            this.uniqueID = uniqueID;
            this.timestamp = timestamp;
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

        public int getUniqueID()
        {
            return uniqueID;
        }

        public void setUniqueID(int uniqueID)
        {
            this.uniqueID = uniqueID;
        }

        public string getTimestamp()
        {
            return timestamp;
        }

        public void setTimestamp(string timestamp)
        {
            this.timestamp = timestamp;
        }
    }

    public interface Ipcs
    {
        void create_replica(int rep_factor, string replica_URL, string whatoperator, string op_id);
    }
    public interface Ipuppet_master
    {
        void log(string log_entry);
    }
    public interface IOperator
    {
        void next_op(string url, string routing);
        void readFile();
        void set_start(string op_spec, int first, string op_id, string logging_level);
        void add_to_inQueue(remoting_interfaces.Tuple tp);
        void set_freeze();
        void set_unfreeze();
        void crash();
        void Interval(int time);
        void Status();
    }
    public interface IfileToOperator
    {
        IList<IList<string>> CustomOperation(IList<string> l);
    }
}
