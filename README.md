# gmc-300

`gmc-300` repository contains a set of C# classes and applications that communicates directly with Geiger counters `GMC-280`, `GMC-300`, `GMC-320` connected to computer with a USB cable.

**Contents**

* Gmc300 class - communicates with `GMC-300` Geiger counter
* Gmc300HistoryParser class - parses `GMC-300` Geiger counter history data
* gmc300parser application - parses `GMC-300` Geiger counter history data
* F.A.Q.
* M4011 tube
* License
* Disclaimer

## Gmc300 class

### Minimal application: Get CPM ticks

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

### Reference

#### Initialization

Use first connected device found:

```
var gmc300 = new Gmc300();
```

Use connected device by specifying port name are baud rate:

```
var gmc300 = new Gmc300("COM6", 57600);
```

#### Get port name

```
var portName = gmc300.PortName;
Console.WriteLine($"Port name: '{bortName}'");
```

#### Get baud rate

```
var baudRate = gmc300.BaudRate;
Console.WriteLine($"Baud rate: '{baudRate}'");
```

#### Get device model (GETVER)

```
var model = gmc300.Model;
Console.WriteLine($"Model: '{model}'");
```

#### Get firmware version (GETVER)

```
var firmwareVersion = gmc300.FirmwareVersion;
Console.WriteLine($"Firmware version: '{firmwareVersion}'");
```

#### Get [counts per minute (CPM)](https://en.wikipedia.org/wiki/Counts_per_minute) (GETCPM)

Returns the detection rate of ionization events per minute.

Remember that this value is inaccurate during first minute of device operation.

```
var cpm = gmc300.GetCpm();
Console.WriteLine($"CPM: {cpm}");
```

To convert CPM to uSv/h, delete CPM by `151.5`:

```
var uSvh = (float)gmc300.GetCpm() / 151.5;
Console.WriteLine($"uSv/h: {uSvh:N2}");
```

#### Get history data (SPIR)

Note that this operation can take up to one minute.

```
var data = gmc300.GetHistoryData();
System.IO.File.WriteAllBytes(@"c:\temp\gmc300.dat", data);
```

If you get exceptions, decrease block size (default is `1024` bytes) or increase read timeout (default is `500` ms):

```
var data = gmc300.GetRawHistoryData(512, 1000);
System.IO.File.WriteAllBytes(@"c:\temp\gmc300.dat", data);
```

#### Get serial number (GETSERIAL)

```
var serialNumber = gmc300.GetSerialNumber();
Console.WriteLine($"Serial number: '{serialNumber}'");
```

#### Get battery voltage (GETVOLT)

```
var volts = (float)gmc300.GetVoltage() / 10;
Console.WriteLine($"Battery voltage: {volts:N1} V");
```

#### Send key to device (KEY)

Emulates key press of device buttons `S1` to `S4`.

Use `Gmc300Keys` enum or values from `0` to `3` (represent hardware keys `S1` to `S4`).

```
gmc300.SendKey(Gmc300Keys.S4); // S4 key, enter menu
gmc300.SendKey(0);             // S1 key, exit menu
```

#### Send multiple keys to device (KEY)

Emulates key press of device buttons `S1` to `S4`.

Use `Gmc300Keys` enum or values from `0` to `3` (represent hardware keys `S1` to `S4`).

```
gmc300.SendKeys(3, 2, 2, 2, 2, 2, 2, 3, 2, 2, 3); // enter menu and show device serial number
System.Threading.Thread.Sleep(5000);              // wait 5 seconds
gmc300.SendKeys(Gmc300Keys.S1, Gmc300Keys.S1);    // exit menu to main screen
```

#### Power off device (POWEROFF)

```
gmc300.PowerOff();
```

#### Power on device (POWERON)

```
gmc300.PowerOn();
```

#### Reboot device (REBOOT)

```
gmc300.Reboot();
```

#### Set device date and time (SETDATETIME)

```
var dateTime = DateTime.Now;
gmc300.SetDateTime(dateTime);
```

#### Get device date and time (GETDATETIME)

```
var deviceDateTime = gmc300.GetDateTime();
Console.WriteLine("Device time: {0}", deviceDateTime.ToString("yyyy.MM.dd HH:mm:ss"));
```

## Gmc300HistoryParser class

### Minimal application: Parse history data

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
            Console.WriteLine($"Geiger counter '{gmc300.Model}' found at port '{gmc300.Port}' (baud rate {gmc300.BaudRate:N0})");

            var historyData = gmc300.GetHistoryData();
            
            var parser = new Gmc300HistoryParser();
            parser.OnCount += (dateTime, dataType, count) => Console.WriteLine("{0} = {1} {2}", dateTime.Format("yyyy.MM.dd HH:mm:ss"), count, 1 == dataType ? "CPS" : "CPM");

            parser.Read(historyData);
        }
    }
}
```

### Reference

## Miscellaneous

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

### License

Distributed under the terms of the [MIT license](https://opensource.org/licenses/MIT).

### Disclaimer

This project is not associated with nor sponsored by [GQ Electronics LLC](http://www.gqelectronicsllc.com/).

All product names, logos, and brands are property of their respective owners and are cited herein for identification purposes only.

```
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
