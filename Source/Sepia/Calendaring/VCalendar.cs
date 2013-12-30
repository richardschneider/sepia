using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class VCalendar
    {
        /// <summary>
        ///   The list of all known calendar components.
        /// </summary>
        /// <remarks>
        ///   The <see cref="ReadIcs"/> method consults the registry to create a newly encountered component.  The new instances
        ///   are added to the <see cref="Components"/> of calendar. 
        /// </remarks>
        /// <example>
        ///   The following example creates a new calendar component.
        ///   <code>TODO</code>
        /// </example>
        public static Dictionary<string, Func<ICalenderComponent>> ComponentRegistry;

        static VCalendar()
        {
            ComponentRegistry = new Dictionary<string, Func<ICalenderComponent>>(StringComparer.InvariantCultureIgnoreCase);
            ComponentRegistry[Component.Names.Alarm] = () => new VAlarm();
            ComponentRegistry[Component.Names.Event] = () => new VEvent();
            ComponentRegistry[Component.Names.FreeBusy] = () => new VFreeBusy();
            ComponentRegistry[Component.Names.Journal] = () => new VJournal();
            ComponentRegistry[Component.Names.TimeZone] = () => new VTimeZone();
            ComponentRegistry[Component.Names.Todo] = () => new VTodo();
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCalendar"/> class with the default values.
        /// </summary>
        /// 
        public VCalendar()
        {
            Components = new List<ICalenderComponent>();
            ProductId = this.GetType().Namespace;
            Version = "2.0";
            Scale = "GREGORIAN";
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCalendar"/> class for the local time zone.
        /// </summary>
        /// <returns>
        ///   A new <see cref="VCalendar"/>.
        /// </returns>
        /// <remarks>
        ///   Adds a <see cref="VTimeZone"/> to the calendar that represents the <see cref="TimeZoneInfo.Local">local time zone</see>.
        /// </remarks>
        public static VCalendar ForLocalTimeZone()
        {
            return new VCalendar { Components = { VTimeZone.FromTimeZoneInfo(TimeZoneInfo.Local) } };
        }

        /// <summary>
        ///   The <see cref="Component">components</see> contained by this calendar.
        /// </summary>
        /// <value>
        ///   A list of <see cref="List{Component}"/>.
        /// </value>
        /// <remarks>
        ///   A component can only exist in one calendar.
        /// </remarks>
        public List<ICalenderComponent> Components { get; set; } 

        /// <summary>
        ///   Specifies the identifier for the product that created the calendar.
        /// </summary>
        /// <value>
        ///   The default value is "Sepia.Calendaring".
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        ///   Specifies the identifier corresponding to the highest version number or the minimum and maximum range of the
        ///   iCalendar specification that is required in order to interpret the calendar object.
        /// </summary>
        /// <value>
        ///   The default value is "2.0".
        /// </value>
        public string Version { get; set; }

        /// <summary>
        ///   Specifies the calendar scale used for the calendar.
        /// </summary>
        /// <value>
        ///   The default value of is "GREGORIAN".
        /// </value>
        public string Scale { get; set; }

        /// <summary>
        ///   Specifies why the calendar was published.
        /// </summary>
        /// <value>
        ///   The default value is <b>null</b>.
        /// </value>
        /// <remarks>
        ///    When used in a MIME message entity, the value of this  property MUST be the same as the Content-Type "method" parameter
        ///    value.  If either the "METHOD" property or the Content-Type "method" parameter is specified, then the other MUST also be
        ///    specified.
        ///    <para>
        ///    See RFC 5546 - "iCalendar Transport-Independent Interoperability Protocol (iTIP)" for more details.
        ///    </para>
        /// </remarks>
        public string Method { get; set; }

        /// <summary>
        ///   The iCalendar representation of the calendar.
        /// </summary>
        /// <seealso cref="WriteIcs"/>
        public override string ToString()
        {
            var ics = new StringWriter();
            using (var writer = IcsWriter.Create(ics))
            {
                this.WriteIcs(writer);
            }
            return ics.ToString();
        }

        /// <summary>
        ///   Read the component properties from the <see cref="IcsReader"/>.
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="IcsReader"/> containing the content of the component.
        /// </param>
        public void ReadIcs(IcsReader reader)
        {
            Guard.IsNotNull(reader, "reader");

            // Must start with "BEGIN:VCALENDAR".
            ContentLine content = reader.ReadContentLine();
            if (content == null)
                throw new CalendarException("Unexpected end of file.");
            if (!(content.Name.ToLowerInvariant() == "begin" && content.Value.ToLowerInvariant() == Component.Names.Calendar.ToLowerInvariant()))
                throw new CalendarException(string.Format("Expected 'BEGIN:VCALENDAR' not '{0}'.", content));

            // Process the content.
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        if (!content.Value.Equals(Component.Names.Calendar, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.Calendar, content));
                        return;
                    case "version": Version = content.Value; break;
                    case "prodid": ProductId = content.Value; break;
                    case "calscale": Scale = content.Value; break;
                    case "method": Method = content.Value; break;
                    case "begin":
                    {
                        Func<ICalenderComponent> factory;
                        ComponentRegistry.TryGetValue(content.Value, out factory);
                        if (factory == null)
                            throw new CalendarException(string.Format("The calendar component '{0}' is not registered.", content.Value));
                        var component = factory.Invoke();
                        component.ReadIcs(reader);
                        Components.Add(component);
                        break;
                    }
                }
            }

            throw new CalendarException("Expected 'END:VCALENDAR' not end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.Calendar);

            // Properties
            ics.WriteContent("calscale", Scale);
            ics.WriteContent("method", Method);
            ics.WriteContent("prodid", ProductId);
            ics.WriteContent("version", Version);

            // Components
            foreach (var component in Components)
            {
                component.WriteIcs(ics);
            }

            ics.WriteEndComponent();
       }

    }
}
