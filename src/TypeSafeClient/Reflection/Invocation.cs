namespace TypeSafeClient.Reflection
{
    using System;

    /// <summary>
    /// Container for method name and parameter values
    /// </summary>
    public class Invocation
    {
        /// <summary> Name of reflected method </summary>
        public string MethodName { get; set; }

        /// <summary> Parameter values </summary>
        public object[] ParameterValues { get; set; }

        /// <summary> Return type, or null if void return </summary>
        public Type ReturnType { get; set; }
    }
}