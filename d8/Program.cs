using System;
using Sprache;

namespace d8
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var result = RegisterInstructionParser.Instruction.Parse("b inc -5");
                Console.WriteLine("{0} {1} {2}", result.Register, result.Operation, result.Amount);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class AllRegisters
    {

    }

    public class RegisterInstruction
    {
        public RegisterInstruction(string register, string operation, int amount, Predicate<AllRegisters> condition)
        {
            
        }
    }

    public enum Operation { Increment, Decrement }
    public enum Comparison { GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual }

    // b inc 5 if a > 1
    // action(register operation number) "if" condition(register comparison-operator number)

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
    }

    public static class RegisterInstructionParser
    {
        static readonly Parser<string> Register = Parse.Identifier(Parse.Letter, Parse.Letter);

        static Parser<Operation> Operator(string input, Operation output)
        {
            return Parse.String(input).Token().Return(output);
        }

        static readonly Parser<Operation> Inc = Operator("inc", Operation.Increment);
        static readonly Parser<Operation> Dec = Operator("dec", Operation.Decrement);

        static readonly Parser<int> Amount =
            from sign in Parse.Char('-').Optional()
            from number in Parse.Number
            select sign.IsDefined ? -Convert.ToInt32(number) : Convert.ToInt32(number);

        public static readonly Parser<Instruction> Instruction =
            from register in Register
            from operation in Inc.XOr(Dec)
            from amount in Amount
            select new Instruction(register, operation, amount);

        static Parser<Comparison> Comparator(string input, Comparison output)
        {
            return Parse.String(input).Token().Return(output);
        }

        static readonly Parser<Comparison> GreaterThan = Comparator(">", Comparison.GreaterThan);
        static readonly Parser<Comparison> GreaterThanOrEqual = Comparator(">=", Comparison.GreaterThanOrEqual);
        static readonly Parser<Comparison> LessThan = Comparator("<", Comparison.LessThan);
        static readonly Parser<Comparison> LessThanOrEqual = Comparator("<=", Comparison.LessThanOrEqual);

        public static readonly Parser<Condition> Condition =
            from register in Register
            from comparison in GreaterThan.XOr(LessThan).XOr(GreaterThanOrEqual).XOr(LessThanOrEqual)
            from amount in Amount
            select new Condition(register, comparison, amount);

        
        // public static readonly Parser<Operation> Operation()
        // {
        //     Parse.Number
        //     var whitespace = Parse.WhiteSpace.Except(Parse.LineEnd);
        //     return from leading in whitespace.Many()
        //            from item in OperationIdentifier
        //            from trailing in whitespace.Many()
        //         select item == 'inc;
        // }
//         public static readonly Parser<IEnumerable<RegisterInstruction>> Assembler = (
//             from register in Register
//             from operation in Operation.Optional()
//             from amount in AsmToken(Comment.SingleLineComment).Optional()
//             from lineTerminator in Parse.LineTerminator
//             select new RegisterInstruction(
//                 label.GetOrDefault(),
//                 instruction.IsEmpty ? null : instruction.Get().Item1,
//                 instruction.IsEmpty ? null : instruction.Get().Item2,
//                 comment.GetOrDefault()
//                 )
// ).XMany().End();

    }
}
