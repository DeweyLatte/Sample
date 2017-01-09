using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummitClientSample
{
    public class Manager
    {
        public Manager()
        { }

        public void Start()
        {
            AsynchronousSocketConnect asc = new AsynchronousSocketConnect("192.168.222.22", 4444);
            asc.SetUpDelegateRec(ReceiveDeletegate);
            asc.Connect();
            asc.Send("YOYO" + char.MinValue);
        }

        public int ReceiveDeletegate(AsynchronousSocketConnect p_conn, String p_msg)
        {
            Console.WriteLine("Recieved msg from " + p_conn.connection.RemoteEndPoint.ToString() + " msg : " + p_msg);
            return 0;
        }
    }
}
