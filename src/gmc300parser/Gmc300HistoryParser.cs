namespace Vurdalakov
{
    using System;
    using System.IO;
    using System.Text;

    public class Gmc300HistoryParser : IDisposable
    {
        private Stream _stream;
        private DateTime _dateTime;
        private Byte _dataType;

        public Gmc300HistoryParser()
        {
        }

        public delegate void OnDateTimeEventHandler(DateTime dateTime, Byte dataType);
        public event OnDateTimeEventHandler OnDateTime = delegate { };

        public delegate void OnCountEventHandler(DateTime dateTime, Byte dataType, Int16 count);
        public event OnCountEventHandler OnCount = delegate { };

        public delegate void OnLabelEventHandler(String label);
        public event OnLabelEventHandler OnLabel = delegate { };

        public void Read(Byte[] historyData)
        {
            using (_stream = new MemoryStream(historyData, false))
            {
                Read();
            }
        }

        public void Read(String historyFileName)
        {
            using (_stream = File.Open(historyFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Read();
            }
        }

        private void Read()
        {
            var is55 = false;
            while (_stream.Position < _stream.Length)
            {
                var b = ReadByte();
                if (0x55 == b)
                {
                    if (is55)
                    {
                        ProcessValue(b);
                    }
                    else
                    {
                        is55 = true;
                    }
                }
                else if (0xAA == b)
                {
                    if (is55)
                    {
                        ProcessTag();
                        is55 = false;
                    }
                    else
                    {
                        ProcessValue(b);
                    }
                }
                else
                {
                    if (is55)
                    {
                        ProcessValue(0x55);
                        is55 = false;
                    }
                    ProcessValue(b);
                }
            }
        }

        private void ProcessTag()
        {
            var tag = ReadByte();
            switch (tag)
            {
                case 0:
                    _dateTime = ReadDateTime();
                    _dataType = Read55AA();
                    OnDateTime?.Invoke(_dateTime, _dataType);
                    break;
                case 1:
                    var data = ReadShort();
                    ProcessValue(data);
                    break;
                case 2:
                    var length = ReadByte();
                    var label = ReadString(length);
                    OnLabel?.Invoke(label);
                    break;
            }
        }

        private void ProcessValue(Int16 value)
        {
            if (0 == _dataType) // history off
            {
                return;
            }

            if (0xFF == value) // no data
            {
                return;
            }

            OnCount(_dateTime, _dataType, value);

            switch (_dataType)
            {
                case 1:
                    _dateTime = _dateTime.AddSeconds(1);
                    break;
                case 2:
                    _dateTime = _dateTime.AddMinutes(1);
                    break;
                case 3:
                    _dateTime = _dateTime.AddHours(1);
                    break;
            }
        }

        private Byte ReadByte()
        {
            return (Byte)_stream.ReadByte();
        }

        private Int16 ReadShort()
        {
            return (Int16)(((Int16)ReadByte() << 8) | ReadByte());
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

        private DateTime ReadDateTime()
        {
            try
            {
                return new DateTime(2000 + ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), DateTimeKind.Local);
            }
            catch
            {
                return new DateTime();
            }
        }

        private Byte Read55AA()
        {
            if ((ReadByte() != 0x55) || (ReadByte() != 0xAA))
            {
                throw new Exception("Not 0x55 0xAA");
            }

            return ReadByte();
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _stream.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
