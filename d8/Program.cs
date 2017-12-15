using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sprache;

namespace d8
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = RegisterInstructionParser.Instance;
            var input = File.ReadAllLines("input.txt");
            var instructions = Array.ConvertAll(
                input,
                line => {
                    Console.WriteLine(line);
                    return parser.Parse(line);
                }
            );
            var registers = new AllRegisters();
            foreach(var instruction in instructions)
                instruction.Execute(registers);
            Console.WriteLine(registers.MaximumValue);
        }
    }

    public enum Operation { Increment, Decrement }
    public enum Comparison { GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, Equal, NotEqual }

    // b inc 5 if a > 1
    // action(register operation number) "if" condition(register comparison-operator number)

    public class AllRegisters
    {
        private Dictionary<string, int> _registers = new Dictionary<string, int>();

        public int MaximumValue 
        {
            get 
            {
                return _registers.Values.Max();
            }
        }

        public int GetRegister(string register)
        {
            if(_registers.TryGetValue(register, out int value))
            {
                return value;
            }
            return 0;
        }

        public void SetRegister(string register, int value)
        {
            if(_registers.ContainsKey(register))
            {
                _registers[register] = value;
            }
            else
            {
                _registers[register] = value;
            }
        }
    }

    public class RegisterInstruction
    {
        public RegisterInstruction(Instruction instruction, Condition condition)
        {
            Instruction = instruction;
            Condition = condition;
        }

        public Instruction Instruction { get; }
        public Condition Condition { get; }

        public void Execute(AllRegisters registers)
        {
            if(Condition.IsSatisfiedBy(registers))
            {
                Instruction.Execute(registers);
            }
        }

        public override string ToString()
        {
            return Instruction.Register + " " + Instruction.Operation + " " + Instruction.Amount + " if " + Condition.Register + " " + Condition.Comparison + " " + Condition.Amount;
        }
    }

    public class Instruction
    {
        public Instruction(string register, Operation operation, int amount)
        {
            Register = register;
            Operation = operation;
            Amount = amount;
        }

        public string Register { get; }
        public Operation Operation { get; }
        public int Amount { get; }

        public void Execute(AllRegisters registers)
        {
            var value = registers.GetRegister(Register);
            switch(Operation)
            {
                case Operation.Increment:
                    registers.SetRegister(Register, value + Amount);
                    break;
                case Operation.Decrement:
                    registers.SetRegister(Register, value - Amount);
                    break;
            }
        }
    }

    public class Condition
    {
        public Condition(string register, Comparison comparison, int amount)
        {
            Register = register;
            Comparison = comparison;
            Amount = amount;
        }

        public string Register { get; }
        public Comparison Comparison { get; }
        public int Amount { get; }

        public bool IsSatisfiedBy(AllRegisters registers)
        {
            var value = registers.GetRegister(Register);
            var result = false;
            switch(Comparison)
            {
                case Comparison.Equal:
                    result = value == Amount;
                    break;
                case Comparison.NotEqual:
                    result = value != Amount;
                    break;
                case Comparison.GreaterThan:
                    result = value > Amount;
                    break;
                case Comparison.GreaterThanOrEqual:
                    result = value >= Amount;
                    break;
                case Comparison.LessThan:
                    result = value < Amount;
                    break;
                case Comparison.LessThanOrEqual:
                    result = value <= Amount;
                    break;
            }
            return result;
        }
    }

    public static class RegisterInstructionParser
    {
        public static readonly Parser<string> Register = Parse.Letter.AtLeastOnce().Token().Text();

        public static Parser<Operation> Operator(string input, Operation output)
        {
            return Parse.String(input).Token().Return(output);
        }

        public static readonly Parser<Operation> Inc = Operator("inc", Operation.Increment);
        public static readonly Parser<Operation> Dec = Operator("dec", Operation.Decrement);

        public static readonly Parser<int> Amount =
            from sign in Parse.Char('-').Optional()
            from number in Parse.Number
            select sign.IsDefined ? -Convert.ToInt32(number) : Convert.ToInt32(number);

        public static readonly Parser<Instruction> Instruction =
            from register in Register
            from operation in Inc.Or(Dec).Token()
            from amount in Amount
            select new Instruction(register, operation, amount);

        public static Parser<Comparison> Comparator(string input, Comparison output)
        {
            return Parse.String(input).Token().Return(output);
        }

        public static readonly Parser<Comparison> Equal = Comparator("==", Comparison.Equal);
        public static readonly Parser<Comparison> NotEqual = Comparator("!=", Comparison.NotEqual);
        public static readonly Parser<Comparison> GreaterThan = Comparator(">", Comparison.GreaterThan);
        public static readonly Parser<Comparison> GreaterThanOrEqual = Comparator(">=", Comparison.GreaterThanOrEqual);
        public static readonly Parser<Comparison> LessThan = Comparator("<", Comparison.LessThan);
        public static readonly Parser<Comparison> LessThanOrEqual = Comparator("<=", Comparison.LessThanOrEqual);

        public static readonly Parser<Condition> Condition =
            from register in Register
            from comparison in Equal.Or(NotEqual).Or(GreaterThanOrEqual).Or(GreaterThan).Or(LessThanOrEqual).Or(LessThan).Token()
            from amount in Amount
            select new Condition(register, comparison, amount);

        public static readonly Parser<RegisterInstruction> Instance =
            from instruction in Instruction
            from literal in Parse.String("if").Token()
            from condition in Condition
            select new RegisterInstruction(instruction, condition);
    }
}
