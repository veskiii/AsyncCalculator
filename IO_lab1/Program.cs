using CalcLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IO_lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculatorAPM server = new CalculatorAPM(IPAddress.Parse("127.0.0.1"), 2048);
            server.Start();
        }
    }
}
