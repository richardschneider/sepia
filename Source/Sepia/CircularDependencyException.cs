using System;
using System.Runtime.Serialization;

namespace Sepia
{
    ///<summary>
    ///  The exception that is thrown when a circular dependency is detected.
    ///</summary>
    // TODO: a wiki reference
    [Serializable]
    public class CircularDependencyException : Exception
    {

        ///<summary>
        ///  Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        ///</summary>
        ///<remarks>
        ///  This constructor initializes the <see cref="Exception.Message"/> property of the new 
        ///  instance to a system-supplied message that describes the error and takes into 
        ///  account the current system culture.
        ///</remarks>
        public CircularDependencyException() : base("A circular dependency has been detected.")
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CircularDependencyException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for the exception.
        /// </param>
        /// <remarks>
        ///   This constructor initializes the <see cref="Exception.Message"/> property of the new 
        ///   instance to the <paramref name="message"/> parameter.
        /// </remarks>
        public CircularDependencyException(string message) : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CircularDependencyException"/> class with a specified error message and
        ///   inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///   The <see cref="Exception"/> that is the cause of the current exception.
        /// </param>
        public CircularDependencyException(
            string message,
            Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CircularDependencyException"/> class with a specified inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="innerException">
        ///   The <see cref="Exception"/> that is the cause of the current exception.
        /// </param>
        public CircularDependencyException(
            Exception innerException)
            : base("A circular dependency has been detected.", innerException)
        {
        }

        ///<summary>
        ///  Initializes a new instance of the <see cref="CircularDependencyException"/> class with serialized data.
        ///</summary>
        ///<param name="info">
        ///  The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        ///</param>
        ///<param name="context">
        ///  The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        ///</param>
        ///<remarks>
        ///  This constructor is called during deserialization to reconstitute the 
        ///  exception object transmitted over a stream. 
        ///</remarks>
        protected CircularDependencyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}


