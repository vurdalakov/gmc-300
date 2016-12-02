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

            //gmc300.PowerOff();
            //gmc300.PowerOn();
            //gmc300.Reboot();
            //return;

            Console.WriteLine($"Model: '{gmc300.Model}'");
            Console.WriteLine($"Firmware version: '{gmc300.FirmwareVersion}'");
            Console.WriteLine($"Serial number: '{gmc300.GetSerialNumber()}'");
            Console.WriteLine("Battery voltage: {0:N1} V", (float)gmc300.GetVoltage() / 10);
            Console.WriteLine("Set current time");
            gmc300.SetDateTime(DateTime.Now);
            Console.WriteLine("Device time: {0}", gmc300.GetDateTime().ToString("yyyy.MM.dd HH:mm:ss"));

            gmc300.SendKey(Gmc300Keys.S4); // S4 key, enter menu
            gmc300.SendKey(0);             // S1 key, exit menu

            gmc300.SendKeys(3, 2, 2, 2, 2, 2, 2, 3, 2, 2, 3); // enter menu and show device serial number
            Thread.Sleep(5000);                               // wait 5 seconds
            gmc300.SendKeys(Gmc300Keys.S1, Gmc300Keys.S1);    // exit menu to main screen

            while (true)
            {
                Console.WriteLine($"CPM: {gmc300.GetCpm()}");
                Thread.Sleep(1000);
            }
        }
    }
}
