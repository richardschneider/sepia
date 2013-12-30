using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   A list of properties associated with a <see cref="Component"/>.
    /// </summary>
    /// <seealso cref="Component"/>
    /// <seealso cref="Component.Properties"/>
    /// <seealso cref="PropertyName"/>
    public class PropertyList : List<ContentLine>
    {
        /// <summary>
        ///   The first <see cref="ContentLine"/> with a <see cref="ContentLine.Name"/> that matches
        ///   the specified <see cref="string"/>.
        /// </summary>
        /// <param name="propertyName">
        ///   The name of the property.  Typically a constant from <see cref="PropertyName"/>.
        /// </param>
        /// <value>
        ///   A <see cref="ContentLine"/>.
        /// </value>
        /// <exception cref="CalendarException">
        ///   On <b>set</b> and the <see cref="ContentLine.Name">ContentLine.Name</see> is not <b>null</b> and does not match the <paramref name="propertyName"/>.
        /// </exception>
        /// <remarks>
        ///   On <b>get</b>, the first <see cref="ContentLine"/> with a <see cref="ContentLine.Name"/> that matches
        ///   the <paramref name="propertyName"/> is returned or <b>null</b> for no matches.
        ///   <para>
        ///   On <b>set</b>, removes all entries with a matching <paramref name="propertyName"/> and then
        ///   adds the <b>value</b>.  If the value's <see cref="ContentLine.Name"/> is not specified (<b>null</b>) then
        ///   it is set <paramref name="propertyName"/>.
        ///   </para>
        /// </remarks>
        /// <seealso cref="PropertyName"/>
        public ContentLine this[string propertyName]
        {
            get { return this.FirstOrDefault((c) => c.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)); }
            set
            {
                this.RemoveAll((c) => c.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                if (value == null)
                    return;

                if (value.Name == null)
                    value.Name = propertyName;
                else if (!value.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                    throw new CalendarException(String.Format("The property name '{0}' and content line name '{1}' do not match.", value.Name, propertyName));

                this.Add(value);
            }
        }

    }
}
