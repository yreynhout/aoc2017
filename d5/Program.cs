using System;
using System.Collections.Generic;
using System.IO;

namespace d5
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var instructions = Array.ConvertAll(lines, line => new Instruction(Convert.ToInt32(line)));
            var message = new Message(instructions);
            Console.WriteLine(message.RunToExit());
        }

        public class Message
        {
            public Message(Instruction[] initialInstructions)
            {
                InitialInstructions = initialInstructions;
            }

            public Instruction[] InitialInstructions { get; }

            public int RunToExit()
            {
                var instructions = new List<Instruction>(InitialInstructions);
                var steps = 0;
                var exit = false;
                var next = 0;
                while(!exit)
                {
                    var previous = next;
                    var instruction = instructions[previous];
                    Console.WriteLine("Instruction at {0} has offset {1}", previous, instruction.Offset);
                    next += instruction.Offset;
                    Console.WriteLine("Jumped to {0}", next);
                    //Console.ReadLine();
                    if(next < 0 || next >= instructions.Count) {
                        exit = true;
                    } else {
                        instructions[previous] = instruction.IncrementOffset();
                    }
                    steps++;
                }
                return steps;
            }
        }

        public class Instruction
        {
            public Instruction(int offset)
            {
                Offset = offset;
            }

            public int Offset { get; }

            public Instruction IncrementOffset()
            {
                return new Instruction(Offset + 1);
            }
        }
    }
}
