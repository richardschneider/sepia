using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   The overall status or confirmation of a <see cref="VTodo"/>.
    /// </summary>
    public class TodoStatus : Tag
    {
        /// <summary>
        ///   The <see cref="VTodo"/> requires some action.
        /// </summary>
        public static TodoStatus NeedsAction = new TodoStatus { Name = "needs-action" };

        /// <summary>
        ///   The <see cref="VTodo"/> is done.
        /// </summary>
        public static TodoStatus Completed = new TodoStatus { Name = "completed" };

        /// <summary>
        ///   The <see cref="VTodo"/> is being worked upon.
        /// </summary>
        public static TodoStatus InProcess = new TodoStatus { Name = "in-process" };

        /// <summary>
        ///   The <see cref="VTodo"/> is cancelled.
        /// </summary>
        public static TodoStatus Cancelled = new TodoStatus { Name = "cancelled" };

        /// <summary>
        ///   Creates a new instance of the <see cref="TodoStatus"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public TodoStatus()
        {
            Authority = "ietf:rfc5545";
        }

    }
}
