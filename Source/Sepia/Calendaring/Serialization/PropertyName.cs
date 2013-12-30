using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   Defines the IANA <see cref="Component.Properties">property (attribute)</see> names of a <see cref="Component"/>
    /// </summary>
    /// <remarks>
    ///   <b>PropertyName</b> contains the IANA key names for a <see cref="Component.Properties">component property</see>.  All
    ///   property names are defined as upper-case as per convention; however these names are case-insensitive as per
    ///   RFC 5545.
    ///   <para>
    ///   See RFC 5545 - Internet Calendaring and Scheduling Core Object Specification
    ///   (iCalendar) for more details.
    ///   </para>
    /// </remarks>
    /// <seealso cref="Component"/>
    public static class PropertyName
    {
        /// <summary>
        ///   Associates a document with a <see cref="Component"/>; IANA name of "ATTACH".
        /// </summary>
        public const string Attachment = "ATTACH";

        /// <summary>
        ///   The calendar scale; represents some type of time division; IANA name of "CALSCALE".
        /// </summary>
        public const string CalendarScale = "CALSCALE";


        /// <summary>
        ///   The access classification for a <see cref="Component"/>; IANA name of "ATTACH".
        /// </summary>
        public const string Classification = "CLASS";

        /// <summary>
        ///   Non-processing information intended to provide a comment to the calendar user; IANA name of "COMMENT".
        /// </summary>
        public const string Comment = "COMMMENT";

        /// <summary>
        ///   A longer description of a <see cref="Component"/>; IANA name of "DESCRIPTION".
        /// </summary>
        public const string Description = "DESCRIPTION";

        /// <summary>
        ///  One or more free or busy time intervals.
        /// </summary>
        public const string FreeBusy = "FREEBUSY";

        /// <summary>
        ///   Identifies the "program" that created the calendar; IANA name of "PROGID";
        /// </summary>
        public const string ProgramId = "PRODID";

        /// <summary>
        ///   Identifies the specification that is required to interpret the calendar; 
        ///   IANA name of "VERSION".
        /// </summary>
        public const string Version = "VERSION";
    }
}
