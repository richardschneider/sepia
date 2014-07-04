using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   The settings for a <see cref="IcsReader"/>
    /// </summary>
    public class IcsReaderSettings
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="IcsReaderSettings"/> class with the default values.
        /// </summary>
        public IcsReaderSettings()
        {
            CloseInput = true;
        }

        /// <summary>
        ///   Determines if the underlying <see cref="System.IO.TextReader"/> is closed when the <see cref="IcsReader"/>
        ///   is closed.
        /// </summary>
        /// <value>
        ///   The default value is <b>true</b>.
        /// </value>
        public bool CloseInput { get; set; }
    }
}
