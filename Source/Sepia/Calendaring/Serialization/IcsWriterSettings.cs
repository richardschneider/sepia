using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   The settings for a <see cref="IcsWriter"/>
    /// </summary>
    public class IcsWriterSettings
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="IcsWriterSettings"/> class with the default values.
        /// </summary>
        public IcsWriterSettings()
        {
            CloseInput = true;
            OctetsPerLine = 75;
        }

        /// <summary>
        ///   Determines if the underlying <see cref="System.IO.TextWriter"/> is closed when the <see cref="IcsWriter"/>
        ///   is closed.
        /// </summary>
        /// <value>
        ///   The default value is <b>true</b>.
        /// </value>
        public bool CloseInput { get; set; }

        /// <summary>
        ///   The maximum octets that a line may contain.
        /// </summary>
        /// <value>
        ///   The default value is 75.
        /// </value>
        public int OctetsPerLine { get; set; }
    }
}
