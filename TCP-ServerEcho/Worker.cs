using System;
using System.IO;
using System.Net.Sockets;

namespace TCP_ServerEcho
{
    internal class Worker
    {
        public Worker()
        {
        }

        public void Start()
        {
            using (TcpClient socket = new TcpClient("localhost", 4646))
            using (StreamReader sr = new StreamReader(socket.GetStream()))
            using (StreamWriter sw = new StreamWriter(socket.GetStream()))
            {
                string str = "Besked la la";

                sw.WriteLine(str);
                sw.Flush();

                string s = sr.ReadLine();
                Console.WriteLine(s);
            }
        }
    }
}