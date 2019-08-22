using System;
using System.Reflection;

[assembly: AssemblyCopyright(""), AssemblyCulture(null), AssemblyDelaySign(true), AssemblyFlags(AssemblyNameFlags.None)]

namespace DSharp.Shell.src
{
    public interface IBaseInt
    {
    }

    public class BaseClass
    {
    }

    public class Lambda<T> : BaseClass, IBaseInt
        where T : Object
    {
        public void Process()
        {
            const bool boolType = false;
            const short shortType = -1;
            const ushort ushortType = 0;
            const int intType = 10000;
            const uint uintType = 1000000;
            const long longType = -1000000000;
            const ulong ulongType = 10000000;
            const float floatType = 1.1;
            const double doubleType = 1.1;
            const decimal decimalType = 1.1;
            const string stringType = "";

            var singleLineActionLambda = new Action(DoSomeWork);
            var multiLineActionStatementLambda = new Action(() =>
            {
                DoSomeWork(null);
            });
            var singleLineActionLambdaWithProperty = new Action<T>(str => DoSomeWork(str));
            var singleLineFuncLambdaWithProperty = new Func<T>(str => DoSomeWork(str));

            var functionNoType = str => DoSomeWork(str);
            var functionNoTypeWithStatement = str =>
            {
                DoSomeWork(str);
            };
            var functionNoTypeWithPropertyType = (T str) => DoSomeWork(str);
            var functionNoTypeWithPropertyTypeWithStatement = (T str) =>
            {
                DoSomeWork(str);
            };

            DoSomeWorkWithLambda((str, obj) =>
            {
                return str + obj.ToString();
            });

            DoSomeWorkWithLambda((str, obj) => str + obj.ToString());
        }

        public string DoSomeWork(T data)
        {

        }

        public T DoSomeWorkWithLambda(Func<T, object, T> func)
        {
            return func.Invoke(null, new object());
        }
    }

    public static class LambdaExtensions
    {
        public static bool IsAwesome<T>(this Lambda<T> instance)
        {

        }
    }
}
