using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace d6
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var banks = new MemoryBanks(
                Array.ConvertAll(
                    input.Replace('\t', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries),
                    blocks => new MemoryBank(Convert.ToInt32(blocks))));
            Console.WriteLine(banks.Reallocate());
        }
    }


    public class MemoryBank
    {
        public MemoryBank(int blocks)
        {
            Blocks = blocks;
        }

        public int Blocks { get; }

        public MemoryBank AddBlock()
        {
            return new MemoryBank(Blocks + 1);
        }

        public MemoryBank ClearBlocks()
        {
            return new MemoryBank(0);
        }

        public string ThumbPrint()
        {
            return Blocks.ToString();
        }
    }

    public class MemoryBanks
    {
        public MemoryBanks(params MemoryBank[] banks)
        {
            Banks = banks.ToList();
        }

        public List<MemoryBank> Banks { get; }

        private string TakeThumbPrint()
        {
            return String.Join(" ", Banks.Select(bank => bank.ThumbPrint()));
        }

        public int Reallocate()
        {
            var thumbprints = new HashSet<string>();
            //thumbprints.Add(String.Join("", Banks.Select(bank => bank.ThumbPrint())));
            var cycles = 0;
            do
            {
                Console.WriteLine("Before: {0}", TakeThumbPrint());
                MemoryBank selected = null;
                var blocks = 0;
                foreach(var bank in Banks)
                {
                    if(bank.Blocks > blocks)
                    {
                        selected = bank;
                        blocks = bank.Blocks;
                    }
                }
                var index = Banks.IndexOf(selected);
                
                Console.WriteLine("Selected bank at {0} with {1} blocks", index, blocks);
                Banks[index] = selected.ClearBlocks();
                while(blocks > 0)
                {
                    index++;
                    if(index == Banks.Count) index = 0;
                    Banks[index] = Banks[index].AddBlock();
                    blocks--;
                }
                cycles++;
                Console.WriteLine("After: {0}", TakeThumbPrint());
                //Console.ReadLine();
            } while(thumbprints.Add(TakeThumbPrint()));
            return cycles;
        }
    }
}
