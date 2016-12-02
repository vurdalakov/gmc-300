namespace Vurdalakov
{
    using System;
    using System.IO;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var gmc300 = new Gmc300();
            Console.WriteLine($"Geiger counter found at port '{gmc300.PortName}' (baud rate {gmc300.BaudRate:N0})");

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

            var now = DateTime.Now;
            var fileName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "gmc300.dat");
            File.WriteAllBytes(fileName, gmc300.GetRawHistoryData());
            Console.WriteLine($"Raw history data saved to '{fileName}' in {(DateTime.Now - now).TotalSeconds:N1} sec");

            while (true)
            {
                var cpm = gmc300.GetCpm();
                var uSvh = (float)cpm / 151.5;
                Console.WriteLine($"CPM: {cpm} or {uSvh:N2} uSv/h");
                Thread.Sleep(1000);
            }
        }
    }
}
