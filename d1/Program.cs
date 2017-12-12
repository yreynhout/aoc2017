using System;
using System.Linq;

namespace d1
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Array.ConvertAll(args[0].ToCharArray(), inputChar => Convert.ToInt32(Char.GetNumericValue(inputChar)));

            var first = input;
            var second = input.Skip(1).Concat(new [] { input[0] });
            var result = first.Zip(second, (int f, int s) => (f == s) ? f : 0).Sum();

            Console.WriteLine(result);
        }
    }
}
