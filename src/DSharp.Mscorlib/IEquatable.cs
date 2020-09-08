﻿using System.Runtime.CompilerServices;

namespace System
{
    public interface IEquatable<T>
    {
        bool Equals(T other);
    }
}
