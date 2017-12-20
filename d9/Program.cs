using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace d9
{
    class Program
    {
        static void Main(string[] args)
        {
            // var lines = new string[]
            // {
            //     "{}", //, 1 group.
            //     "{{{}}}", //, 3 groups.
            //     "{{},{}}", //, also 3 groups.
            //     "{{{},{},{{}}}}", //, 6 groups.
            //     "{<{},{},{{}}>}", //, 1 group (which itself contains garbage).
            //     "{<a>,<a>,<a>,<a>}", //, 1 group.
            //     "{{<a>},{<a>},{<a>},{<a>}}", //, 5 groups.
            //     "{{<!>},{<!>},{<!>},{<a>}}", //, 2 groups (since all but the last > are canceled).
            //     "<>",//, empty garbage.
            //     "<random characters>",//, garbage containing random characters.
            //     "<<<<>",//, because the extra < are ignored.
            //     "<{!>}>",//, because the first > is canceled.
            //     "<!!>",//, because the second ! is canceled, allowing the > to terminate the garbage.
            //     "<!!!>>",//, because the second ! and the first > are canceled.
            //     "<{o\"i!a,<{i<a>",//, which ends at the first >.
            //     "{}",//, score of 1.
            //     "{{{}}}",//, score of 1 + 2 + 3 = 6.
            //     "{{},{}}",//, score of 1 + 2 + 2 = 5.
            //     "{{{},{},{{}}}}",//, score of 1 + 2 + 3 + 3 + 3 + 4 = 16.
            //     "{<a>,<a>,<a>,<a>}",//, score of 1.
            //     "{{<ab>},{<ab>},{<ab>},{<ab>}}",//, score of 1 + 2 + 2 + 2 + 2 = 9.
            //     "{{<!!>},{<!!>},{<!!>},{<!!>}}",//, score of 1 + 2 + 2 + 2 + 2 = 9.
            //     "{{<a!>},{<a!>},{<a!>},{<ab>}}",//, score of 1 + 2 = 3.
            // };
            var lines = File.ReadAllLines("input.txt");
            foreach(var line in lines)
            {
                Console.WriteLine("Line:{0}", line);
                var context = new Context();
                IState initial = new InitialState(context);
                line.Aggregate(initial, (before, @char) => 
                {
                    var after = before.Process(@char);
                    //Console.WriteLine("Processing: {0} - before: {1} - after {2}", @char, before.GetType().Name, after.GetType().Name);
                    return after;
                });
                Console.WriteLine("Result:{0}", context.GroupCount);
            }
        }
    }

    public interface IState
    {
        IState Process(char @char);
    }

    public class InitialState : IState
    {
        public InitialState(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public IState Process(char @char)
        {
            IState next = null;
            switch (@char)
            {
                case '{':
                    next = new GroupState(Context.Push(this));
                    break;
                case '<':
                    next = new GarbageState(Context.Push(this));
                    break;
                default:
                    next = this;
                    break;
            }
            return next;
        }
    }

    public class GroupState : IState
    {
        public GroupState(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public IState Process(char @char)
        {
            IState next = null;
            switch (@char)
            {
                case '<':
                    next = new GarbageState(Context.Push(this));
                    break;
                case '}':
                    Context.GroupCount += Context.GetGroupScore() + 1;
                    next = Context.Pop();
                    break;
                case '{':
                    next = new GroupState(Context.Push(this));
                    break;
                default:
                    next = this;
                    break;
            }
            return next;
        }
        
    }

    public class GarbageState : IState
    {
        public GarbageState(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public IState Process(char @char)
        {
            IState next = null;
            switch (@char)
            {
                case '!':
                    next = new EscapingGarbageState(Context.Push(this));
                    break;
                case '>':
                    next = Context.Pop();
                    break;
                default:
                    next = this;
                    break;
            }
            return next;
        }
    }

    public class Context
    {
        private readonly Stack<IState> _history;

        public Context()
        {
            _history = new Stack<IState>();
        }

        public IState Pop()
        {
            return _history.Pop();
        }

        public Context Push(IState state)
        {
            _history.Push(state);
            return this;
        }

        public int GetGroupScore()
        {
            return _history.Count(state => state is GroupState);
        }

        public int GroupCount { get; set; }
    }
    
    public class EscapingGarbageState : IState
    {
        public EscapingGarbageState(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public IState Process(char @char)
        {
            return Context.Pop();
        }
    }
}
