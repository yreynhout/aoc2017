using System;
using System.IO;
using System.Linq;

namespace d2
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);
            var rows = Array.ConvertAll(
                input,
                line => 
                    line.Replace('\t', ' ')
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(candidate => Convert.ToInt32(candidate))
                        .ToArray()
                );
            var result = rows.Aggregate(0, (current, row) => current + row.Max() - row.Min());
            Console.WriteLine(result);
        }
    }
}
