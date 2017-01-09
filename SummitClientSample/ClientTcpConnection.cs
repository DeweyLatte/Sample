using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SummitClientSample
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketConnect
    {
        private int serverPort = 0;
        private string remoteIpAddress = null;
        public Socket connection = null;
        private Func<AsynchronousSocketConnect, String, int> ManagerFuncRec;
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketConnect(string p_ipAddress, int p_port)
        {
            serverPort = p_port;
            remoteIpAddress = p_ipAddress;
        }

        public int Connect()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            IPAddress ipAddress = IPAddress.Parse(remoteIpAddress);


            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, serverPort);

            // Create a TCP/IP socket.
            connection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                connection.Connect(remoteEndPoint);
                // Get the socket that handles the client request.


               // int val = ManagerFuncAcc.Invoke(this, listener, handler);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = connection;
                connection.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return 0;
            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();

        }


        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                //if (content.IndexOf("<EOF>") > -1)
                if (content.IndexOf(Char.MinValue) > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    //   content.Length, content);


                    int val = ManagerFuncRec.Invoke(this, content);
                    // Echo the data back to the client.
                    //Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            connection.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), connection);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);


                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal void SetUpDelegateRec(Func<AsynchronousSocketConnect, String, int> ReceiveDeletegate)
        {
            ManagerFuncRec = ReceiveDeletegate;
        }
    }
}
