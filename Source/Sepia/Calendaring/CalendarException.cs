using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   An error encountered in a iCalendar.
    /// </summary>
    [Serializable]
    public class CalendarException : Exception
    {
        ///<summary>
        ///  Initializes a new instance of the <see cref="CalendarException"/> class.
        ///</summary>
        ///<remarks>
        ///  This constructor initializes the <see cref="Exception.Message"/> property of the new 
        ///  instance to a system-supplied message that describes the error and takes into 
        ///  account the current system culture.
        ///</remarks>
        public CalendarException()
            : base()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CalendarException"/> class with 
        ///   the specified <see cref="Exception.Message"/>.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for the exception.
        /// </param>
        /// <remarks>
        ///   This constructor initializes the <see cref="Exception.Message"/> property to <paramref name="message"/>.
        /// </remarks>
        public CalendarException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CalendarException"/> class with 
        ///   the specified <see cref="Exception.Message"/> and <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///   The <see cref="Exception"/> that is the cause of the current exception.
        /// </param>
        /// <remarks>
        ///   This constructor initializes the <see cref="Exception.Message"/> property to <paramref name="message"/> and
        ///   the <see cref="Exception.InnerException"/> to <paramref name="innerException"/>.
        /// </remarks>
        public CalendarException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
