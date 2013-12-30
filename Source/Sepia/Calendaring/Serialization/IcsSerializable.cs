using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   Provides custom formatting for iCalendar serialization and deserialization.
    /// </summary>
    /// <remarks>
    ///   A calendar component supports the iCalendar data format (<c>.ics</c> files). It can be deserialised and serialised with 
    ///   the <see cref="ReadIcs"/> and <see cref="WriteIcs"/> methods, respectively.
    /// </remarks>
    public interface IcsSerializable
    {
        /// <summary>
        ///   Read the properties from the <see cref="IcsReader"/>.
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="IcsReader"/> containing the content of the object.
        /// </param>
        /// <remarks>
        ///   <note>Applications MUST ignore x-param and iana-param values they don't recognize.</note>
        /// </remarks>
        void ReadIcs(IcsReader reader);

        /// <summary>
        ///   Writes the object to the <see cref="IcsWriter"/>.
        /// </summary>
        /// <param name="writer">
        ///   That <see cref="IcsWriter"/> that receives the object's iCalendar representation.
        /// </param>
        void WriteIcs(IcsWriter writer);
    }
}
