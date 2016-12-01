namespace Vurdalakov
{
    using System;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var gmc300 = new Gmc300();
            Console.WriteLine($"Geiger counter found at port '{gmc300.Port}' (baud rate {gmc300.BaudRate:N0})");

            Console.WriteLine($"Model: '{gmc300.Model}'");
            Console.WriteLine($"Firmware version: '{gmc300.FirmwareVersion}'");

            while (true)
            {
                Console.WriteLine($"CPM: {gmc300.GetCpm()}");
                Thread.Sleep(1000);
            }
        }
    }
}
