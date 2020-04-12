using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ClearScriptBug
{
    public static class Program
    {
        public static void Main()
        {
            Execute(new[] { 1, 2, 3 }, "context.Count()");
            Execute(new List<int> { 1, 2, 3 }, "context.Count()");
            Execute(new List<int> { 1, 2, 3 }, "context.ToArray().Count()");
            Execute(new List<int> { 1, 2, 3 }, "context.AsEnumerable().Count()");
            Execute(new List<int> { 1, 2, 3 }, "Enumerable.Count(context)");

            Execute(new[] { 1, 2, 3 }, "context.Sum()");
            Execute(new[] { new Object { X = 1 }, new Object { X = 2 }, new Object { X = 3 } }, "context.Sum(new func(arg=>arg.X))");
            Execute(new[] { new Object { X = 1 }, new Object { X = 2 }, new Object { X = 3 } }, "Enumerable.Sum(context, new func(arg=>arg.X))");
            Execute(new[] { new Object { X = 1 }, new Object { X = 2 }, new Object { X = 3 } }, "Enumerable.Sum(context, arg=>arg.X)");
        }

        public class Object
        {
            public int X { get; set; }
        }

        private static void Execute(object context, string script)
        {
            try
            {
                using var engine = new V8ScriptEngine
                {
                    AllowReflection = true
                };

                engine.AddHostType(typeof(Enumerable));
                engine.AddHostObject("context", context);
                engine.AddHostObject("host", new HostFunctions());
                engine.AddHostType("func", typeof(Func<Object, int>));

                Console.WriteLine(engine.Evaluate(script));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message} Error testing {context} [{context.GetType().Name}]");
            }
        }
    }
}
