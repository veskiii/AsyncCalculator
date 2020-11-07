using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CalcLib
{
    /// <summary>
    /// This is an abstract class for Calculator Servers.
    /// </summary>
    public abstract class Calculator
    {
        #region Fields
        IPAddress iPAddress;
        int port;
        int buffer_size = 1024;
        bool running;
        TcpListener tcpListener;
        TcpClient tcpClient;
        NetworkStream stream;
        #endregion
        #region Properties
        /// <summary>
        /// This property gives access to the IP address of a server instance. Property can't be changed while the Server is running.
        /// </summary>
        public IPAddress IPAddress
        {
            get => iPAddress; set
            {
                if (!running) iPAddress = value;
                else throw new Exception("Nie mozna zmienic adresu IP kiedy serwer jest uruchomiony");
            }
        }

        /// <summary>
        /// This property gives access to the port of a server instance. Property can't be changed while the Server is running. 
        /// Setting invalid port numbers will cause an exception.
        /// </summary>
        public int Port
        {
            get => port; set
            {
                int tmp = port;
                if (!running) port = value; else throw new Exception("Nie mozna zmienic portu kiedy serwer jest uruchomiony");
                if (!checkPort())
                {
                    port = tmp;
                    throw new Exception("Bledna wartosc portu");
                }
            }
        }
        
        /// <summary>
        /// This property gives access to the buffer size of a server instance. Property can't be changed while the Server is running.
        /// Setinng invalid size numbers will cause an exception.
        /// </summary>
        public int Buffer_size
        {
            get => buffer_size; set
            {
                if (value < 0 || value > 1024 * 1024 * 64) throw new Exception("Bledny rozmiar pakietu");
                if (!running) buffer_size = value; else throw new Exception("Nie mozna zmienic rozmiaru pakietu kiedy serwer jest uruchomiony");
            }
        }

        protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }

        protected TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }

        protected NetworkStream Stream { get => stream; set => stream = value; }

        #endregion
        #region Constructors
        /// <summary>
        /// A default constructor. It doesn't start the server. Invalid port numbers will throw an exception.
        /// </summary>
        /// <param name="IP">IP address of the server instance.</param>
        /// <param name="port">Port number of the server instance.</param>
        public Calculator(IPAddress IP, int port)
        {
            running = false;
            IPAddress = IP;
            Port = port;
            if (!checkPort())
            {
                Port = 8000;
                throw new Exception("Bledna wartosc portu, ustawiono 8000");
            }
        }
        #endregion
        #region Functions
        /// <summary>
        /// This function will return false if Port is set to a value lower than 1024 or higher than 49151.
        /// </summary>
        /// <returns>An information wether the set Port value is valid.</returns>
        protected bool checkPort()
        {
            if (port < 1024 || port > 49151) return false;
            return true;
        }

        /// <summary>
        /// This function starts the listener.
        /// </summary>
        protected void StartListening()
        {
            TcpListener = new TcpListener(IPAddress, Port);
            TcpListener.Start();
        }

        /// <summary>
        /// This function waits for the Client connection.
        /// </summary>
        protected abstract void AcceptClient();

        /// <summary>
        /// This function transmits the data between server and client.
        /// </summary>
        /// <param name="stream"></param>
        protected abstract void BeginDataTransmission(NetworkStream stream);

        /// <summary>
        /// This function fires off the default server behaviour. It interrupts the program.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// This function calculates expression for float input values.
        /// </summary>
        /// <param name="op">Math operation sign.</param>
        /// <returns>The result of the expression.</returns>
        private static float calculate(float x, char op, float y)
        {
            if (op == '+') return x + y;
            else if (op == '-') return x - y;
            else if (op == '/') return x / y;
            else if (op == '*') return x * y;
            return 0;
        }

        /// <summary>
        /// This function calculates expression for integer input values.
        /// </summary>
        /// <param name="op">Math operation sign.</param>
        /// <returns>The result of the expression.</returns>
        private static int calculate(int x, char op, int y)
        {
            if (op == '+') return x + y;
            else if (op == '-') return x - y;
            else if (op == '*') return x * y;
            return 0;
        }

        /// <summary>
        /// This function parses the string expression.
        /// </summary>
        /// <param name="input">Mathematical expression.</param>
        /// <returns>The result of the expression.</returns>
        protected static string parseInput(string input)
        {
            string[] words = input.Split();
            char op = words[1][0];
            string response = "";
            if(words[0] == "sqrt")
            {
                response = Math.Sqrt(double.Parse(words[1])).ToString() + "\r\n";
            }
            else if(words[0].Contains('.') || words[2].Contains('.') || op == '/')
            {
                float result = calculate(float.Parse(words[0]), op, float.Parse(words[2]));
                response = result.ToString() + "\r\n";
            }
            else
            {
                int result = calculate(int.Parse(words[0]), op, int.Parse(words[2]));
                response = result.ToString() + "\r\n";
            }
            return response;
        }
        #endregion
    }
}
