namespace Vurdalakov
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: gmc300parser <file name>");
            }

            var fileName = args[0];

            using (var parser = new Gmc300HistoryParser())
            {
                parser.OnDateTime += (dateTime, dataType)           => Console.WriteLine($"DATETIME {dateTime:yyyy.MM.dd HH:mm:ss} type {dataType}");
                parser.OnCount +=    (dateTime, dataType, count)    => Console.WriteLine($"COUNT    {dateTime:yyyy.MM.dd HH:mm:ss} {count} {GetType(dataType)}");
                parser.OnLabel +=    (label)                        => Console.WriteLine($"LABEL    {label}");

                parser.Read(fileName);
            }
        }

        static private String GetType(Byte dataType)
        {
            return 1 == dataType ? "CPS" : "CPM";
        }
    }
}
