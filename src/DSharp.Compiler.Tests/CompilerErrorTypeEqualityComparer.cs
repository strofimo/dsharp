using System.Collections.Generic;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Tests
{
    public class CompilerErrorTypeEqualityComparer : IEqualityComparer<IError>
    {
        public bool Equals(IError x, IError y)
        {
            return x.GetType() == y.GetType();
        }

        public int GetHashCode(IError obj)
        {
            return obj.GetType().FullName.GetHashCode();
        }
    }
}
