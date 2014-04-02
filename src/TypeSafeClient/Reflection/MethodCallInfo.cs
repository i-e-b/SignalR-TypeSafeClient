namespace TypeSafeClient.Reflection
{
    using System;

    /// <summary>
    /// Container for method name and parameters
    /// </summary>
    public class MethodCallInfo
    {
        /// <summary> Name of reflected method </summary>
        public string MethodName { get; set; }

        /// <summary> Parameter values </summary>
        public Type[] ParameterTypes { get; set; }
    }
}