using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace d4
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("input.txt");
            var result = lines.Aggregate(0, (current, line) =>
            {
                var phrases = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if(new HashSet<string>(phrases).Count == phrases.Length)
                    return current + 1;
                return current;
            });
            Console.WriteLine(result);
        }
    }
}
