# gmc-300

`gmc-300` repository contains a set of C# classes and applications that communicates directly with Geiger counters `GMC-280`, `GMC-300`, `GMC-320` connected to computer with a USB cable.

### Usage Examples

#### Minimal application: Get CPM ticks

```
namespace Vurdalakov
{
    using System;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var gmc300 = new Gmc300(); // will find the device automatically
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
```

#### Get battery voltage (GETVOLT)

```
var volts = (float)gmc300.GetVoltage() / 10;
Console.WriteLine($"Battery voltage: {volts:N1} V");
```

#### Get serial number (GETSERIAL)

```
var serialNumber = gmc300.GetSerialNumber();
Console.WriteLine($"Serial number: '{serialNumber}'");
```

#### Get device date and time (GETDATETIME)

```
var deviceDateTime = gmc300.GetDateTime();
Console.WriteLine("Device time: {0}", deviceDateTime.ToString("yyyy.MM.dd HH:mm:ss"));
```

### F.A.Q.

* What Geiger tube is used in GMC-300?

Both GMC-300 and GMC-320 use `M4011` tube. It is possible to replace it with another tube like `SBM-20`.

* Do I need to install drivers?

No drivers are required for Windows 10. When connected, GMC-300 is recognized as `USB2.0-Serial` USB device and `USB-SERIAL CH340` COM port.

### M4011 tube

Made in China. Used for both beta and gamma radiation detetion.

There is no official M4011 specification available. Below you can find information collected from various Internet sources.

Glass tube made (new) in China. Claimed to be more sensitive than the SBM-20. Although I once got high readings, it now seems similar to the SMB-20 - not too impressive, especially at it's price. It's possible this tube may be light sensitive which would explain the initial high counts but that is not confirmed by other users. It is reported that a good CPM to uSv/h conversion rate for this tube is 151.5.

https://sites.google.com/site/diygeigercounter/gm-tubes-supported

Working voltage: 380-450 V
Working current: 0,015-0,02 mA
Sensivity to gamma radiation: 0.1 MeV
Own background: 0,2 pulses/s
Working temperature range: -50...+60 С
Length: 88 mm
Diameter: 10 mm

http://www.ebay.com/itm/CPT-081-Geiger-Counter-Tube-M4011-High-Sensitivie-Beta-Gamma-Radiation-Detection-/120997840180

Tin oxide Cathode, Coaxial cylindrical thin shell structure(Wall density 50±10cg/cm2),Application of pulse type halogen tube
application temperature:-40°C~55°C
Could be used for :γRay 20mR/h~120mR/h  
               and β Ray in range  100~1800 ChangingIndex/minutes·CM2 soft β Ray
               (Both beta and gamma radiation detetion)
Working Voltage: 380-450V 
Working Current: 0,015-0,02 mA
Sensivity to Gamma Radiation: 0.1 MeV
Own Background: 0,2 Pulses/s
Length:  88 mm
Diameter: 10 mm

https://www.aliexpress.com/item/The-Tube-for-Geiger-Counter-Kit-The-tube-for-Nuclear-Radiation-Detector-GM-Tube/32336147853.html

### Other GMC-300 clients

* [Official clients from GQ Electronics](http://www.gqelectronicsllc.com/comersus/store/download.asp)
* [GMC 300 Python Client](https://github.com/stilldavid/gmc-300-python)
