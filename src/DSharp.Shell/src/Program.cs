using System;
using System.Collections.Generic;

namespace DSharp.Shell.src
{
    public enum MyAwesomeEnum
    {
        Number1 = 1,
        Number2,
        Number3,
        Number4
    }

    public delegate void TestDelegate(object instance, params string[] args);

    public delegate T DoSomethingFunc<T>(T inVar, Func<T, T> mutator);

    public class Base { }

    public class Program
    {
        private const int NUM = 10000;

        private static TOut ParseGenerics<TOut, TIn>(TIn inValue, Func<TIn, TOut> converter)
        {
            return converter.Invoke(inValue);
        }

        public static void Main(string[] args)
        {
            var method = new Func<int>(() => 1);
            var value = method.Invoke();

            var result = ParseGenerics(1.1, (db) => (int)db);

            int[] arr = new int[0];
            var arr2 = new int[] { 1 };

            ToEnumerable<Implementation, Base>(new Implementation[2]);
            var imp = new ImplementationOfAbstract();

            List<int> values = new List<int>()
            {
                imp.Grab(1),
                imp.Grab(2),
                imp.Grab(3),
                imp.Grab(4),
            };
            values.Add(imp.Grab(5));

            for (int i = 6; i < NUM; i++)
            {
                var val = imp.Grab(i);
                values.Add(val);
                Console.WriteLine(i + " = " + val);
            }

            DoSomethingFunc<string> doSomethingFunc = LocalMethod;
            Console.WriteLine(doSomethingFunc.Invoke("Fred", (v1) => v1 + "DoIt"));
        }

        private static string LocalMethod(string inVar, Func<string, string> appender)
        {
            return inVar + "_" + appender.Invoke("");
        }

        class Implementation : Base { }

        public static IEnumerable<TBase> ToEnumerable<T, TBase>(ICollection<T> args)
            where T : TBase
            where TBase : Base
        {
            Type baseType = typeof(TBase);
            Type tType = typeof(T);
            Type argsType = args.GetType();

            // this explodes in normal C#
            // just to check that IEnumerable<T> is available properly
            return (IEnumerable<T>)(object)args;
        }
    }

    public abstract class AbstractClass : Base
    {
        public abstract void DoSomething();

        public virtual int Grab<T>(T not)
        {
            return 0;
        }
    }

    public class ImplementationOfAbstract : AbstractClass
    {
        public override void DoSomething()
        {
            int index = 0;
            do
            {
                if (index == 5)
                    continue;
            } while (++index < 10);

            var str = "";
            switch (str)
            {
                case "me":
                    break;
                default:
                    return;
            }
        }

        public override int Grab<T>(T not)
        {
            int val = 0;
            if (not is int)
            {
                int notAsInt = (int)(object)not;
                val = ++notAsInt;
            }

            val *= 2;
            val += 3;
            val /= 4;
            val -= 2;

            val <<= 2;
            val >>= 1;
            return (base.Grab(not) + ((val * 2 / 4) << -(val))) >> 1;
        }
    }
}
