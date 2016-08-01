using System;

#if WIN81
using System.Reflection;
#endif

namespace TypeSafeClient.Extensions
{
	/// <summary>
	/// The <see cref="System.Type"/> extension methods.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Gets a value indicating whether the Type is an interface; that is, not a class or a value type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if the specified type is interface; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsInterface(this Type type)
		{
#if WIN81
			return type.GetTypeInfo().IsInterface;
#else
			return type.IsInterface;
#endif
		}

		/// <summary>
		/// Gets a value indicating whether the Type is an abstract class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if the specified type is abstract class; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsAbstract(this Type type)
		{
#if WIN81
			return type.GetTypeInfo().IsAbstract;
#else
			return type.IsAbstract;
#endif
		}
	}
}
