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
    public class CalculatorAPM : Calculator
    {
        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public CalculatorAPM(IPAddress IP, int port) : base(IP, port)
        {

        }
        public override void Start()
        {
            StartListening();
            AcceptClient();
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();
                Stream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);

            }
        }

        private void TransmissionCallback(IAsyncResult ar)
        {

        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            byte[] buffer2 = new byte[Buffer_size];
            string converted, response;
            while (true)
            {
                try
                {
                    int message_size = stream.Read(buffer, 0, Buffer_size);
                    converted = Encoding.ASCII.GetString(buffer, 0, message_size);
                    if (message_size > 2)
                    {
                        response = parseInput(converted);
                        buffer2 = Encoding.ASCII.GetBytes(response);
                        stream.Write(buffer2, 0, response.Length);
                    }
                }
                catch (IOException e)
                {
                    break;
                }
            }
        }
    }
}
