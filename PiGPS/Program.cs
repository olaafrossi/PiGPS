// Created: 2022|01|21
// Modified: 2022|01|21
// PiGPS|Program.cs|PiGPS
// Olaaf Rossi

using System;
using System.IO.Ports;
using System.Threading;

// scp -r C:\Dev\PiGPS\PiGPS\bin\Release\net5.0\publish\* pi@cm01:Desktop/Deployment/
// https://endjin.com/blog/2019/09/passwordless-ssh-from-windows-10-to-raspberry-pi
// scp id_ed25519.pub pi@cm01:~\.ssh\authorized_keys
// https://docs.microsoft.com/en-us/visualstudio/debugger/remote-debugging-dotnet-core-linux-with-ssh?view=vs-2019

namespace PiGPS
{
    internal class Program
    {
        private static readonly SerialPort _portRead = new("/dev/ttyUSB2", 115200, Parity.None);
        private static readonly SerialPort _portWrite = new("/dev/ttyUSB1", 115200, Parity.None);

        private static void Main(string[] args)
        {
            ResetGps();

            NmeaInterpreter gps = new();

            gps.PositionReceived += Gps_PositionReceived;
            gps.FixObtained += Gps_FixObtained;

            while (true)
            {
                string read = _portRead.ReadLine();
                Console.WriteLine(read);
                Console.WriteLine(gps.Parse(read));
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(500);
            }
        }

        private static void ResetGps()
        {
            _portWrite.Open();
            Thread.Sleep(100);
            _portWrite.Write("AT+QGPS=1\\r");
            Thread.Sleep(100);
            _portWrite.Close();
            _portRead.Open();
        }

        private static void Gps_FixObtained()
        {
            Console.WriteLine("Fix Obtained");
        }

        private static void Gps_PositionReceived(string latitude, string longitude)
        {
            Console.WriteLine($"lat {latitude} long {longitude}");
        }
    }
}