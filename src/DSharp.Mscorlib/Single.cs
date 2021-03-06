﻿using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// The float data type which is mapped to the Number type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Number")]
    public struct Single
    {
        [ScriptAlias("parseFloat")]
        public extern static float Parse(string s);

        /// <summary>
        /// Returns a string containing the value represented in exponential notation.
        /// </summary>
        /// <returns>The exponential representation</returns>
        public extern string ToExponential();

        /// <summary>
        /// Returns a string containing the value represented in exponential notation.
        /// </summary>
        /// <param name="fractionDigits">The number of digits after the decimal point (0 - 20)</param>
        /// <returns>The exponential representation</returns>
        public extern string ToExponential(int fractionDigits);

        /// <summary>
        /// Returns a string representing the value in fixed-point notation.
        /// </summary>
        /// <returns>The fixed-point notation</returns>
        public extern string ToFixed();

        /// <summary>
        /// Returns a string representing the value in fixed-point notation.
        /// </summary>
        /// <param name="fractionDigits">The number of digits after the decimal point from 0 - 20</param>
        /// <returns>The fixed-point notation</returns>
        public extern string ToFixed(int fractionDigits);

        /// <summary>
        /// Returns a string containing the value represented either in exponential or
        /// fixed-point notation with a specified number of digits.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        public extern string ToPrecision();

        /// <summary>
        /// Returns a string containing the value represented either in exponential or
        /// fixed-point notation with a specified number of digits.
        /// </summary>
        /// <param name="precision">The number of significant digits (in the range 1 to 21)</param>
        /// <returns>The string representation of the value.</returns>
        public extern string ToPrecision(int precision);

        //TODO: Move to Number type
        public extern static implicit operator Number(float i);
    }
}
