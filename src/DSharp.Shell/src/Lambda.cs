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
}
