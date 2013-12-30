using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   Represents an iCalendar object that can be persisted.
    /// </summary>
    /// <remarks>
    ///   A <see cref="Component"/> is a container that can be persisted and contains
    ///   a collection of <see cref="Properties"/> and other <see cref="Components"/>
    ///   <para>
    ///   All components contain <see cref="Properties"/> which are attributes that apply to the object as a
    ///   whole. RFC 5545 specified <b>Properties</b> are implemented as specific property methods.  An individual
    ///   property is represented as a <see cref="ContentLine"/>.
    ///   </para>
    /// </remarks>
    public class Component
    {
        /// <summary>
        ///   Defines the IANA names for all Calendar and Scheduling <see cref="Component">components</see>.
        /// </summary>
        /// <remarks>
        ///   See RFC 5545 - Internet Calendaring and Scheduling Core Object Specification
        ///   (iCalendar) for more details.
        /// </remarks>
        public class Names
        {
            /// <summary>
            ///   The <see cref="Component.Name"/> of a calendar.
            /// </summary>
            public const string Calendar = "VCALENDAR";

            /// <summary>
            ///   The <see cref="Component.Name"/> of an event.
            /// </summary>
            public const string Event = "VEVENT";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a todo.
            /// </summary>
            public const string Todo = "VTODO";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a journal entry.
            /// </summary>
            public const string Journal = "VJOURNAL";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a free/busy entry.
            /// </summary>
            public const string FreeBusy = "VFREEBUSY";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a time zone.
            /// </summary>
            public const string TimeZone = "VTIMEZONE";

            /// <summary>
            ///   The <see cref="Component.Name"/> of an alarm.
            /// </summary>
            public const string Alarm = "VALARM";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a "Winter" seasonal time zone.
            ///   <see cref="TimeZone"/>.
            /// </summary>
            public const string Standard = "STANDARD";

            /// <summary>
            ///   The <see cref="Component.Name"/> of a "Summer" seasonal time zone.
            /// </summary>
            public const string DayLight = "DAYLIGHT";
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Component"/> class with the specified <see cref="Names">name</see>.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Names">name of the component</see> as specified by RFC 5545 - Internet Calendaring and Scheduling Core Object Specification
        ///   (iCalendar).
        /// </param>
        protected Component(string name)
        {
            Name = name;
            Properties = new PropertyList();
            Components = new List<Component>();
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Component"/> class from the specified
        ///   IANA <see cref="Name"/> with no default <see cref="Properties"/>.
        /// </summary>
        /// <param name="ianaName">
        ///   The IANA assigned <see cref="Name"/>.
        /// </param>
        /// <returns>
        ///   A new instance of the <see cref="Component"/> class that implements the
        ///   <paramref name="ianaName"/>.
        /// </returns>
        /// <exception cref="CalendarException">
        ///   <paramref name="ianaName"/> is not known nor experimental.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///   <b>Create</b> is designed to be used by <see cref="IcsReader"/>.
        ///   </para>
        /// </remarks>
        public static Component Create(string ianaName)
        {
            var component = new Component(ianaName);
            component.Properties.Clear();
            return component;
        }

        /// <summary>
        ///   The IANA name of the component.
        /// </summary>
        /// <value>
        ///   One of the <see cref="Component.Names">Compnent.Names</see> constants.
        /// </value>
        /// <remarks>
        ///   Component names are specified by RFC 5545 - Internet Calendaring and Scheduling Core Object Specification
        ///   (iCalendar).
        /// </remarks>
        public string Name { get; private set; }

        /// <summary>
        ///   Attributes that apply to the component as a  whole.
        /// </summary>
        /// <value>
        ///   A <see cref="PropertyList"/>.
        /// </value>
        public PropertyList Properties { get; private set; }

        /// <summary>
        ///   Gets or sets the string value of a case-insensitive property name.
        /// </summary>
        /// <param name="propertyName">
        ///   The case-insensitive name of the <see cref="Properties">property</see>.
        /// </param>
        /// <returns>
        ///   The first string <see cref="ContentLine.Value"/> for the <paramref name="propertyName"/> or <b>null</b>
        ///   if the <paramref name="propertyName"/> does not exist.
        /// </returns>
        public string this[string propertyName]
        {
            get 
            {
                var content = Properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                if (content == null)
                    return null;
                return content.Value;
            }
            set 
            {
                var property = Properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                if (property == null)
                    Properties.Add(new ContentLine() { Name = propertyName, Value = value});
                else
                    property.Value = value;
            }
        }

        /// <summary>
        ///   The <see cref="Component">components</see> contained by this <see cref="Component"/>.
        /// </summary>
        /// <value>
        ///   A list of <see cref="IList{Component}"/>.
        /// </value>
        public List<Component> Components { get; set; }

    }
}
