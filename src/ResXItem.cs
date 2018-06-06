// ResXItem.cs
// Script#/Core
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Diagnostics;

namespace DSharp
{
    internal sealed class ResXItem
    {
        private string value;
        private string comment;

        public ResXItem(string name, string value, string comment)
        {
            Debug.Assert(String.IsNullOrEmpty(name) == false);

            Name = name;
            this.value = value;
            this.comment = comment;
        }

        public string Comment
        {
            get
            {
                if (comment == null)
                {
                    return String.Empty;
                }
                return comment;
            }
        }

        public string Name { get; private set; }

        public string Value
        {
            get
            {
                if (value == null)
                {
                    return String.Empty;
                }
                return value;
            }
        }
    }
}