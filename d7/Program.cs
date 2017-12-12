using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace d7
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var messages = Array.ConvertAll(lines, line => TowerMessage.Parse(line));
            var towers = Array.ConvertAll(messages, message => new Tower(message.Name, message.Weight));
            var lookup = towers.ToDictionary(tower => tower.Name);
            for(var index = 0; index < towers.Length; index++)
            {
                var tower = towers[index];
                var message = messages[index];
                foreach(var name in message.NameOfChildTowers)
                {
                    if(lookup.TryGetValue(name, out Tower child))
                    {
                        child.HeldUpBy(tower);
                    }
                }
            }
            var max = 0;
            Tower bottom = null;
            foreach(var tower in towers)
            {
                if(tower.Depth > max)
                {
                    bottom = tower;
                    max = tower.Depth;
                }
            }
            if(bottom != null) Console.WriteLine(bottom.Name);
        }
    }

    public class TowerMessage
    {
        public TowerMessage(string name, int weight, string[] nameOfChildTowers)
        {
            Name = name;
            Weight = weight;
            NameOfChildTowers = nameOfChildTowers;
        }

        public string Name { get; }
        public int Weight { get; }
        public string[] NameOfChildTowers { get; }

        public static TowerMessage Parse(string line)
        {
            var pass1 = line.Split("->");
            var pass2 = pass1[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var name = pass2[0];
            var weight = Convert.ToInt32(pass2[1].Substring(1, pass2[1].Length - 2));
            if(pass1.Length == 2)
            {
                var pass3 = pass1[1].Replace(" ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
                return new TowerMessage(name, weight, pass3);
            }
            return new TowerMessage(name, weight, new string[0]);
        }
    }

    public class Tower
    {
        private List<Tower> children;

        public Tower(string name, int weigth)
        {
            Name = name;
            Weigth = weigth;
            children = new List<Tower>();
            computeDepth = true;
        }

        public string Name { get; }
        public int Weigth { get; }
        public Tower Parent { get; private set; }
        public Tower[] Children => children.ToArray();

        public void HeldUpBy(Tower other)
        {
            Parent = other;

            other.IsHoldingUp(this);
        }

        private void IsHoldingUp(Tower other)
        {
            children.Add(other);
            computeDepth = true;
        }

        public int Depth {
            get {
                if(computeDepth)
                {
                    if(Children.Length == 0)
                    {
                        depth = 1;
                    } 
                    else
                    {
                        depth = Children.Max(child => child.Depth) + 1;
                    }
                    computeDepth = false;
                }
                return depth;
            }
        }

        private bool computeDepth;
        private int depth;
    }
}
