using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   Defines the IANA <see cref="ContentLine.Parameters">parameter</see> names.
    /// </summary>
    /// <remarks>
    ///   <b>ParameterName</b> contains the IANA key names for a <see cref="ContentLine.Parameters">parameter</see>.  All
    ///   parameters names are defined as upper-case as per convention; however these names are case-insensitive as per
    ///   RFC 5545.
    ///   <para>
    ///   See RFC 5545 - Internet Calendaring and Scheduling Core Object Specification
    ///   (iCalendar) for more details.
    ///   </para>
    /// </remarks>
    /// <seealso cref="ContentLine"/>
    public static class ParameterName
    {
        /// <summary>
        ///   An alternate text representation (URI) for the property value; IANA name of "ALTREP".
        /// </summary>
        public const string AlternativeRepresentation = "ALTREP";

        /// <summary>
        ///   An alternative representation for a property value.
        /// </summary>
        public const string AlternativeId = "ALTID";

        /// <summary>
        ///   The common name associated with the calendar user; IANA name of "CN".
        /// </summary>
        public const string CommonName = "CN";

        /// <summary>
        ///   Identifies the type of a calendar user; IANA name of "CUTYPE".
        /// </summary>
        public const string CalendarUserType = "CUTYPE";


        /// <summary>
        ///   The calendar user(s) that have delegated their
        ///   to another calendar user; IANA name of "DELEGATED-FROM".
        /// </summary>
        public const string Delegators = "DELEGATED-FROM";

        /// <summary>
        ///   The calendar user(s) to whom the calendar user
        ///   specified by the property has delegated participation; IANA name of "DELEGATED-TO".
        /// </summary>
        public const string Delegatee = "DELEGATED-TO";

        /// <summary>
        ///   Reference to a directory entry associated with the calendar user; IANA name of "DIR".
        /// </summary>
        public const string DirectoryEntryReference = "DIR";

        /// <summary>
        ///   Specifies an alternate inline encoding; IANA name of "ENCODING".
        /// </summary>
        public const string InlineEncoding = "ENCODING";

        /// <summary>
        ///   Specifies the content type of a referenced object; IANA name of "FMTTYPE".
        /// </summary>
        public const string FormatType = "FMTTYPE";

        /// <summary>
        ///   Specifies the free/busy time type; IANA name of "FBTYPE".
        /// </summary>
        public const string FreeBusyTimeType = "FBTYPE";

        /// <summary>
        ///   Specifies the language for text values; IANA name of "LANGUAGE".
        /// </summary>
        public const string Language = "LANGUAGE";

        /// <summary>
        ///   Specifies the group or list membership of the calendar user; IANA name of "MEMBER".
        /// </summary>
        public const string GroupMembership = "MEMBER";

        /// <summary>
        ///   Specifies the participation status of the calendar user; IANA name of "PARTSTAT".
        /// </summary>
        public const string ParticipationStatus = "PARTSTAT";

        /// <summary>
        ///   Property identifier.
        /// </summary>
        public const string PropertyId = "PID";

        /// <summary>
        ///   Specifies the effective range of a recurrence; IANA name of "RANGE".
        /// </summary>
        public const string Range = "RANGE";

        /// <summary>
        ///   Specifies the relationship of the alarm trigger with respect to the start or end of the <see cref="Component"/>;
        ///   IANA name of "RELATED".
        /// </summary>
        public const string AlarmTriggerRelationship = "RELATED";

        /// <summary>
        ///   Specifies the hierarchical relationship between two <see cref="Component">components</see>; IANA name of "RELTYPE".
        /// </summary>
        public const string RelationshipType = "RELTYPE";

        /// <summary>
        ///   Specifies the participation role for the calendar user; IANA name of "ROLE".
        /// </summary>
        public const string Role = "ROLE";

        /// <summary>
        ///   Determines if there is an expectation of an RSVP from the calendar user; IANA name of "RSVP";
        /// </summary>
        public const string RsvpExpectation = "RSVP";

        /// <summary>
        ///   The identifier for the time zone definition for a time component in the property value
        /// </summary>
        public const string TimeZoneId = "TZID";

        /// <summary>
        ///   A classification.
        /// </summary>
        public const string Type = "TYPE";

        /// <summary>
        ///   The preferred ordering (1 - 100).
        /// </summary>
        public const string Preference = "PREF";
    }
}
