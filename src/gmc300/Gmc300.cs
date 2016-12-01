namespace Vurdalakov
{
    using System;
    using System.IO.Ports;
    using System.Text;

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

            throw new Exception("GMC-300 device is not found.");
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
            _serialPort.WriteLine("<GETVER>>");

            return ReadString(14);
        }

        public Int32 GetCpm()
        {
            _serialPort.WriteLine("<GETCPM>>");

            var cpm = _serialPort.ReadByte() << 8;
            return cpm | _serialPort.ReadByte();
        }

        public Int32 GetVoltage()
        {
            _serialPort.WriteLine("<GETVOLT>>");

            return _serialPort.ReadByte();
        }

        public String GetSerialNumber()
        {
            _serialPort.WriteLine("<GETSERIAL>>");

            return ReadHexString(7);
        }

        private String ReadString(Int32 length)
        {
            var stringBuilder = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append((char)_serialPort.ReadByte());
            }

            return stringBuilder.ToString();
        }

        private String ReadHexString(Int32 length)
        {
            var stringBuilder = new StringBuilder(length * 2);
            for (var i = 0; i < length; i++)
            {
                stringBuilder.AppendFormat("{0:X2}", _serialPort.ReadByte());
            }

            return stringBuilder.ToString();
        }
    }
}
