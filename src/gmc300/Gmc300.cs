namespace Vurdalakov
{
    using System;
    using System.IO.Ports;
    using System.Text;
    using System.Threading;

    public enum Gmc300Keys { S1 = 0, S2 = 1, S3 = 2, S4 = 3 }

    public class Gmc300
    {
        private SerialPort _serialPort;

        public String Port { get; private set; }
        public Int32 BaudRate { get; private set; }
        public String Model { get; private set; }
        public String FirmwareVersion { get; private set; }

        public Gmc300()
        {
            Int32[] baudRates = { 115200, 57600, 38400, 28800, 19200, 14400, 9600, 4800, 2400, 1200 };
            foreach (var baudRate in baudRates)
            {
                foreach (var port in SerialPort.GetPortNames())
                {
                    try
                    {
                        using (var serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One))
                        {
                            serialPort.ReadTimeout = 500;
                            serialPort.Open();

                            serialPort.WriteLine("<GETVER>>");
                            for (var i = 0; i < 14; i++)
                            {
                                serialPort.ReadByte();
                            }
                        }

                        Open(port, baudRate);
                        return;
                    }
                    catch { }
                }
            }

            throw new Exception("Geiger counter is not found.");
        }

        public Gmc300(String port, Int32 baudRate)
        {
            Open(port, baudRate);
        }

        private void Open(String port, Int32 baudRate)
        {
            _serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Open();

            Port = port;
            BaudRate = baudRate;

            var version = GetVersion();
            Model = version.Substring(0, 7);
            FirmwareVersion = version.Substring(7);
        }

        public String GetVersion()
        {
            WriteLine("GETVER");

            return ReadString(14);
        }

        public Int32 GetCpm()
        {
            WriteLine("GETCPM");

            return (ReadByte() << 8) | ReadByte();
        }

        public Int32 GetVoltage()
        {
            WriteLine("GETVOLT");

            return ReadByte();
        }

        public void SendKey(Int32 key)
        {
            WriteLine("KEY", key);
            Thread.Sleep(500);
        }

        public void SendKey(Gmc300Keys key)
        {
            SendKey((Int32)key);
        }

        public void SendKeys(Int32 key, params Int32[] keys)
        {
            SendKey(key);

            for (var i = 0; i < keys.Length; i++)
            {
                SendKey(keys[i]);
            }
        }

        public void SendKeys(Gmc300Keys key, params Gmc300Keys[] keys)
        {
            SendKey(key);

            for (var i = 0; i < keys.Length; i++)
            {
                SendKey(keys[i]);
            }
        }

        public String GetSerialNumber()
        {
            WriteLine("GETSERIAL");

            return ReadHexString(7);
        }

        public void PowerOff()
        {
            WriteLine("POWEROFF");
        }

        public void PowerOn()
        {
            WriteLine("POWERON");
        }

        public void Reboot()
        {
            WriteLine("REBOOT");
        }

        public void SetDateTime(DateTime dateTime)
        {
            WriteLine("SETDATETIME",
                 dateTime.Year % 100, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0xAA);

            ReadAaByte();
        }

        public DateTime GetDateTime()
        {
            WriteLine("GETDATETIME");

            var deviceDateTime = new DateTime(2000 + ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), DateTimeKind.Local);
            ReadAaByte();

            return deviceDateTime;
        }

        private void WriteLine(String command, params Int32[] parameters)
        {
            _serialPort.Write("<" + command);

            if (parameters.Length > 0)
            {
                var bytes = Array.ConvertAll(parameters, b => (Byte)b);

                _serialPort.Write(bytes, 0, bytes.Length);
            }

            _serialPort.WriteLine(">>");
        }

        private Byte ReadByte()
        {
            var b = (Byte)_serialPort.ReadByte();
            //Console.WriteLine($"{b:X2}");
            return b;
        }

        private String ReadString(Int32 length)
        {
            var stringBuilder = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append((char)ReadByte());
            }

            return stringBuilder.ToString();
        }

        private String ReadHexString(Int32 length)
        {
            var stringBuilder = new StringBuilder(length * 2);
            for (var i = 0; i < length; i++)
            {
                stringBuilder.AppendFormat("{0:X2}", ReadByte());
            }

            return stringBuilder.ToString();
        }

        private void ReadAaByte()
        {
            var b = ReadByte();

            if (b != 0xAA)
            {
                throw new Exception($"Received byte 0x{b:X2}, expected byte  0xAA");
            }
        }
    }
}
