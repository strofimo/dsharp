using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace TypeTests {

    public class MyClass {

        public object this[int index]
        {
            get { return index; }
            set { }
        }

        public int Getter { get; set; }

        public void Method()
        {

        }

        public MyClass() {
            MemberInfo[] members = typeof(MyClass).GetMembers();
            Assert(members[0].Name == "get_Item");
            Assert(members[1].Name == "set_Item");
            Assert(members[2].Name == "Getter");
            Assert(((PropertyInfo)members[2]).PropertyType == typeof(int));
            Assert(members[3].Name == "Method");
            Assert(((MethodInfo)members[3]).ReturnType == null);
        }

        private static void Assert(bool assertion)
        {

        }
    }
}
