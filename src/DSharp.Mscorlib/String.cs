using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Equivalent to the String type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class String
    {
        /// <summary>
        /// An empty zero-length string.
        /// </summary>
        public static readonly string Empty = "";

        /// <summary>
        /// The number of characters in the string.
        /// </summary>
        [ScriptField]
        public extern int Length { get; }

        /// <summary>
        /// Retrieves the character at the specified position.
        /// </summary>
        /// <param name="index">The specified 0-based position.</param>
        /// <returns>The character within the string.</returns>
        [ScriptField]
        public extern char this[int index] { get; }

        /// <summary>
        /// Retrieves the character at the specified position.
        /// </summary>
        /// <param name="index">The specified 0-based position.</param>
        /// <returns>The character within the string.</returns>
        public extern char CharAt(int index);

        /// <summary>
        /// Retrieves the character code of the character at the specified position.
        /// </summary>
        /// <param name="index">The specified 0-based position.</param>
        /// <returns>The character code of the character within the string.</returns>
        public extern int CharCodeAt(int index);

        [DSharpScriptMemberName("compareStrings")]
        public extern static int Compare(string s1, string s2);

        [DSharpScriptMemberName("compareStrings")]
        public extern static int Compare(string s1, string s2, bool ignoreCase);

        [DSharpScriptMemberName("string")]
        public extern static string Concat(string s1, string s2);

        [DSharpScriptMemberName("string")]
        public extern static string Concat(string s1, string s2, string s3);

        [DSharpScriptMemberName("string")]
        public extern static string Concat(string s1, string s2, string s3, string s4);

        /// <summary>
        /// Concatenates a set of individual strings into a single string.
        /// </summary>
        /// <param name="strings">The sequence of strings</param>
        /// <returns>The concatenated string.</returns>
        [DSharpScriptMemberName("string")]
        public extern static string Concat(params string[] strings);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DSharpScriptMemberName("string")]
        public extern static string Concat(object o1, object o2);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DSharpScriptMemberName("string")]
        public extern static string Concat(object o1, object o2, object o3);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DSharpScriptMemberName("string")]
        public extern static string Concat(object o1, object o2, object o3, object o4);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DSharpScriptMemberName("string")]
        public extern static string Concat(params object[] o);

        /// <summary>
        /// Returns the unencoded version of a complete encoded URI.
        /// </summary>
        /// <returns>The unencoded string.</returns>
        [ScriptAlias("decodeURI")]
        public extern string DecodeUri();

        /// <summary>
        /// Returns the unencoded version of a single part or component of an encoded URI.
        /// </summary>
        /// <returns>The unencoded string.</returns>
        [ScriptAlias("decodeURIComponent")]
        public extern string DecodeUriComponent();

        /// <summary>
        /// Encodes the complete URI.
        /// </summary>
        /// <returns>The encoded string.</returns>
        [ScriptAlias("encodeURI")]
        public extern string EncodeUri();

        /// <summary>
        /// Encodes a single part or component of a URI.
        /// </summary>
        /// <returns>The encoded string.</returns>
        [ScriptAlias("encodeURIComponent")]
        public extern string EncodeUriComponent();

        /// <summary>
        /// Determines if the string ends with the specified character.
        /// </summary>
        /// <param name="ch">The character to test for.</param>
        /// <returns>true if the string ends with the character; false otherwise.</returns>
        [DSharpScriptMemberName("endsWith")]
        public extern bool EndsWith(char ch);

        /// <summary>
        /// Determines if the string ends with the specified substring or suffix.
        /// </summary>
        /// <param name="suffix">The string to test for.</param>
        /// <returns>true if the string ends with the suffix; false otherwise.</returns>
        [DSharpScriptMemberName("endsWith")]
        public extern bool EndsWith(string suffix);

        /// <summary>
        /// Encodes a string by replacing punctuation, spaces etc. with their escaped equivalents.
        /// </summary>
        /// <returns>The escaped string.</returns>
        [ScriptAlias("escape")]
        public extern string Escape();

        [DSharpScriptMemberName("format")]
        public extern static string Format(string format, params object[] values);

        [DSharpScriptMemberName("format")]
        public extern static string Format(CultureInfo culture, string format, params object[] values);

        [DSharpScriptMemberName("string")]
        public extern static string FromChar(char ch, int count);

        public extern static string FromCharCode(int charCode);

        public extern static string FromCharCode(params int[] charCodes);

        public extern int IndexOf(char ch);

        public extern int IndexOf(string subString);

        public extern int IndexOf(char ch, int startIndex);

        public extern int IndexOf(string subString, int startIndex);

        [DSharpScriptMemberName("insertString")]
        public extern string Insert(int index, string value);

        [DSharpScriptMemberName("emptyString")]
        public extern static bool IsNullOrEmpty(string s);

        [DSharpScriptMemberName("whitespace")]
        public extern static bool IsNullOrWhiteSpace(string s);

        public extern int LastIndexOf(Char ch);

        public extern int LastIndexOf(string subString);

        public extern int LastIndexOf(char ch, int startIndex);

        public extern int LastIndexOf(string subString, int startIndex);

        public extern string[] Match(RegExp regex);

        [DSharpScriptMemberName("padLeft")]
        public extern string PadLeft(int totalWidth);

        [DSharpScriptMemberName("padLeft")]
        public extern string PadLeft(int totalWidth, char ch);

        [DSharpScriptMemberName("padRight")]
        public extern string PadRight(int totalWidth);

        [DSharpScriptMemberName("padRight")]
        public extern string PadRight(int totalWidth, char ch);

        [DSharpScriptMemberName("removeString")]
        public extern string Remove(int index);

        [DSharpScriptMemberName("removeString")]
        public extern string Remove(int index, int count);

        [DSharpScriptMemberName("replaceString")]
        public extern string Replace(string oldText, string replaceText);

        [ScriptName("replace")]
        public extern string ReplaceFirst(string oldText, string replaceText);

        [ScriptName("replace")]
        public extern string ReplaceRegex(RegExp regex, string replaceText);

        [ScriptName("replace")]
        public extern string ReplaceRegex(RegExp regex, StringReplaceCallback callback);

        public extern int Search(RegExp regex);

        public extern string[] Split(char ch);

        public extern string[] Split(string separator);

        public extern string[] Split(char ch, int limit);

        public extern string[] Split(string separator, int limit);

        public extern string[] Split(RegExp regex);

        public extern string[] Split(RegExp regex, int limit);

        [DSharpScriptMemberName("startsWith")]
        public extern bool StartsWith(char ch);

        [DSharpScriptMemberName("startsWith")]
        public extern bool StartsWith(string prefix);

        public extern string Substr(int startIndex);

        public extern string Substr(int startIndex, int length);

        public extern string Substring(int startIndex);

        public extern string Substring(int startIndex, int endIndex);

        public extern string ToLocaleLowerCase();

        public extern string ToLocaleUpperCase();

        [Obsolete("ToLowerCase() should not be used, switch to ToLower()")]
        public extern string ToLowerCase();

        [ScriptName("toLowerCase")]
        public extern string ToLower();

        [Obsolete("ToUpperCase() should not be used, switch to ToUpper()")]
        public extern string ToUpperCase();

        [ScriptName("toUpperCase")]
        public extern string ToUpper();

        [DSharpScriptMemberName("trim")]
        public extern string Trim();

        [DSharpScriptMemberName("trim")]
        public extern string Trim(char[] trimCharacters);

        [DSharpScriptMemberName("trimEnd")]
        public extern string TrimEnd();

        [DSharpScriptMemberName("trimEnd")]
        public extern string TrimEnd(char[] trimCharacters);

        [DSharpScriptMemberName("trimStart")]
        public extern string TrimStart();

        [DSharpScriptMemberName("trimStart")]
        public extern string TrimStart(char[] trimCharacters);

        /// <summary>
        /// Decodes a string by replacing escaped parts with their equivalent textual representation.
        /// </summary>
        /// <returns>The unescaped string.</returns>
        [ScriptAlias("unescape")]
        public extern string Unescape();

        /// <internalonly />
        public extern static bool operator ==(string s1, string s2);

        /// <internalonly />
        public extern static bool operator !=(string s1, string s2);
    }
}
