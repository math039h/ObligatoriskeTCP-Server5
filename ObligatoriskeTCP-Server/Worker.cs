using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ObligatoriskBibloteksProject;

namespace ObligatoriskeTCP_Server
{
    class Worker
    {
        private static List<Bog> BogListe = new List<Bog>()
        {
            new Bog("olivers bog", "brink", 213, "1234567891113"),
            new Bog("oliver bog", "brin", 243, "2234567891113"),
            new Bog("olirs bog", "brink", 523, "3234567891113"),
            new Bog("ivers og", "bink", 23, "4234567891113")
        };

        public Worker()
        {
        }


        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 4646);
            server.Start();

            while (true)
            {
                TcpClient socket = server.AcceptTcpClient();

                Task.Run(
                    () =>
                    {
                        TcpClient tmpsocket = socket;
                        DoClient(tmpsocket);
                    }
                );
            }
        }

        private void DoClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();

            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                while (true)
                {
                    PrintNoget(sw);
                    string input = sr.ReadLine();

                    if (input == "HentAlle")
                    {
                        sr.ReadLine();
                        sw.WriteLine(getall());
                        sw.Flush();
                    }

                    if (input == "Hent")
                    {
                        string isbn = sr.ReadLine();
                        sw.WriteLine(getbook(isbn));
                        sw.Flush();
                    }

                    if (input == "Gem")
                    {
                        string jsonbog = sr.ReadLine();
                        save(jsonbog);
                    }

                }
                string str = sr.ReadLine();
                sw.WriteLine(str);
                sw.Flush();
            }
            socket.Close();
        }

        private void PrintNoget(StreamWriter sw)
        {
            sw.WriteLine("hej, velkommen til \"bog\"-serveren.");
            sw.WriteLine("");
            sw.WriteLine("Du har nu 3 valgmuligheder. Skriv HentAlle for at se alle vores bøger");
            sw.WriteLine("skriv Hent efterfulgt af et ISBN-nummer, for at se hvilke info vi har om den bog");
            sw.WriteLine("skriv Gem efterfulgt af et JSON-format af et bog-objekt, for at gemme enny bog i databasen");
            sw.WriteLine("Eksempel: ");
            sw.WriteLine("{Author:\"bob\",Isbn:\"1231231231234\",\"Pages\":\"25\",\"Title\":\"Book of the ages\"}");
            sw.Flush();
        }

        private string getbook(string isbn)
        {
            Bog bog = BogListe.Find((bog1 => bog1.Lsbn13 == isbn));
            return (bog == null) ? "Jeg kan ikke finde den bog" : JsonConvert.SerializeObject(bog);
        }

        private string getall()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Bog bog in BogListe)
            {
                sb.Append(JsonConvert.SerializeObject(bog));
            }
            return sb.ToString();
        }

        private void save(string bog)
        {
            Bog b = JsonConvert.DeserializeObject<Bog>(bog);
            BogListe.Add(b);
        }
    }
}