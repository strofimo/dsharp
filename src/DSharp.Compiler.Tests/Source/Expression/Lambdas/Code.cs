using System;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace ExpressionTests {

    public class Test {

        private Func<bool> getFalse;

        public Test() {
            this.getFalse = () => false;
            Func<int, int> addOne = a => a + 1;
            Func<int, int, int> sum = (a, b) => a + b;
            Action doNothing = () => { };
            Action<bool> doSomething = a => {
                if(!a)
                {
                    return;
                };
            };
        }
    }
}
