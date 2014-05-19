using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Prevents bad data from entering the system.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        ///   Demand that the argument is not null.
        /// </summary>
        /// <typeparam name="T">
        ///   A reference value (<c>class</c> in C#).
        /// </typeparam>
        /// <param name="x">The argument to test for nullability.</param>
        /// <param name="name">The argument's name.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="x"/> is <b>null</b>.</exception>
        public static void IsNotNull<T>(T x, string name) where T: class
        {
            if (x == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        ///   Demand that the <see cref="string">string argument</see> is not null and is not empty and does not exist of whitespace.
        /// </summary>
        /// <param name="x">The argument to test.</param>
        /// <param name="name">The argument's name.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="x"/> does not meet the specified criteria.</exception>
        public static void IsNotNullOrWhiteSpace(string x, string name)
        {
            if (string.IsNullOrWhiteSpace(x))
                throw new ArgumentNullException(name);
        }


        /// <summary>
        ///   Demand that the property has not been set yet.
        /// </summary>
        /// <param name="x">The argument to test for mutability.</param>
        /// <param name="name">The argument's name</param>
        /// <exception cref="InvalidOperationException">When <paramref name="x"/> cannot be changed.</exception>
        /// <remarks>
        ///   Eric has coined this <see ref="http://blogs.msdn.com/b/ericlippert/archive/2007/11/13/immutability-in-c-part-one-kinds-of-immutability.aspx">Popsicle Immutability</see>.
        /// </remarks>
        public static void IsMutable<T>(T x, string name) 
        {
            if (!EqualityComparer<T>.Default.Equals(x, default(T)))
                throw new InvalidOperationException(string.Format("'{0}' cannot be changed because it already has the value '{1}'.", name, x));
        }

        /// <summary>
        ///   Requires that the precondition is true.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="name">The argument's name</param>
        /// <param name="message">A description of the condition.</param>
        /// <exception cref="ArgumentException">When <paramref name="condition"/> is not <b>true</b>.</exception>
        public static void Require(bool condition, string name, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message, name);
            }
        }

        /// <summary>
        ///   Requires that the precondition is true.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">A description of the condition.</param>
        /// <exception cref="InvalidOperationException">When <paramref name="condition"/> is not <b>true</b>.</exception>
        public static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        ///   When the predicate is valid, require that the condition is true.
        /// </summary>
        /// <param name="predicate">Determines if the <paramref name="condition"/> is checked.</param>
        /// <param name="condition">The condition to check.</param>
        /// <param name="name">The argument's name</param>
        /// <param name="message">A description of the condition.</param>
        /// <exception cref="ArgumentException">When <paramref name="condition"/> is not <b>true</b>.</exception>
        public static void RequireWhen(bool predicate, bool condition, string name, string message)
        {
            if (predicate && !condition)
            {
                throw new ArgumentException(message, name);
            }
        }

    }
}
