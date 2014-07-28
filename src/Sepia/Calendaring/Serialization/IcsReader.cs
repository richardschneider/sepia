using System;
using System.IO;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///    A reader that provides fast, non-cached, forward-only access to iCalendar data.
    /// </summary>
    /// <remarks>
    ///   <b>IcsReader</b> provides forward-only, read-only access to a stream of iCalendar data. The <b>IcsReader</b> class conforms to the 
    ///   IETF RFC 5455 - Internet Calendaring and Scheduling Core Object Specification
    /// </remarks>
    /// <example>
    ///   <code>
    ///   using (var reader = IcsReader.Create(File.OpenText(@"sample.ics")))
    ///   {
    ///       ContentLine line;
    ///       while ((line = reader.ReadContentLine()) != null)
    ///       {
    ///           Console.WriteLine("{0} : {1}", line.Name, line.Value);
    ///       }
    ///   }
    ///   </code>
    /// </example>
    public class IcsReader : IDisposable
    {
        TextReader reader;
        IcsReaderSettings settings;

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsReader"/> with the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="TextReader"/> from which to read iCalendar data. All iCalendar data is encoded as UTF-8.
        /// </param>
        public static IcsReader Create(TextReader reader)
        {
            return new IcsReader(reader, new IcsReaderSettings());
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsReader"/> with the specified <see cref="TextReader"/>
        ///   and <see cref="IcsReaderSettings"/>
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="TextReader"/> from which to read iCalendar data. All iCalendar data is encoded as UTF-8.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="IcsReaderSettings"/> for the reader.
        /// </param>
        public static IcsReader Create(TextReader reader, IcsReaderSettings settings)
        {
            return new IcsReader(reader, settings);
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsReader"/> with the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="TextReader"/> from which to read iCalendar data. All iCalendar data is encoded as UTF-8.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="IcsReaderSettings"/> for the reader.
        /// </param>
        IcsReader(TextReader reader, IcsReaderSettings settings)
        {
            this.reader = reader;
            this.settings = settings;
        }

        /// <summary>
        ///   Closes the reader.
        /// </summary>
        /// <remarks>
        ///   If <see cref="IcsReaderSettings.CloseInput"/> is <b>true</b>, then the underlying <see cref="TextReader"/>
        ///   is <see cref="TextReader.Close">closed</see>.
        /// </remarks>
        public void Dispose()
        {
            if (reader != null && settings != null && settings.CloseInput)
                reader.Close();
        }

        /// <summary>
        ///   Read a <see cref="Component"/>.
        /// </summary>
        /// <returns></returns>
        public Component ReadComponent()
        {
            return ReadComponent(ReadContentLine());
        }

        Component ReadComponent(ContentLine content)
        {
            try
            {
                if (content == null)
                    return null;
                if (!content.Name.Equals("begin", StringComparison.InvariantCultureIgnoreCase))
                    throw new CalendarException(String.Format("Expected 'BEGIN' not '{0}'.", content.Name));
                var component = Component.Create(content.Value);
                while ((content = ReadContentLine()) != null)
                {
                    switch (content.Name.ToLowerInvariant())
                    {
                        case "end":
                            if (!content.Value.Equals(component.Name, StringComparison.InvariantCultureIgnoreCase))
                                throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", component.Name, content));
                            return component;
                        case "begin":
                            component.Components.Add(ReadComponent(content));
                            break;
                        default:
                            component.Properties.Add(content);
                            break;
                    }
                }
                throw new CalendarException(String.Format("Missing 'END:{0}'.", component.Name));
            }
            catch (CalendarException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CalendarException("Syntax error.", e);
            }
        }


        /// <summary>
        ///   Returns the <see cref="ContentLine"/> representation of the next unfolded calendar line.
        /// </summary>
        /// <returns>
        ///   The <see cref="ContentLine"/> representation of the next unfolded calendar line or <b>null</b> if no more data is present.
        /// </returns>
        /// <remarks>
        ///   A content line has the form: <c><see cref="ContentLine.Name"/> *(";" <see cref="ContentLine.Parameters">parameter</see> ) ":" <see cref="ContentLine.Value"/> CRLF</c>
        ///   <para>
        ///   A long line can be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public ContentLine ReadContentLine()
        {

            // Unfold the line(s).
            string line = reader.ReadLine();
            if (line == null)
                return null;
            while (true)
            {
                switch ((char)reader.Peek())
                {
                    case ' ':
                    case '\t':
                        reader.Read();
                        line += reader.ReadLine();
                        continue;
                }
                break;
            }
            var s = new StringReader(line);

            var content = new ContentLine();
            content.Name = ReadName(s);
            while (true)
            {
                var c = s.Read();
                switch (c)
                {
                    case ';':
                        var name = ReadName(s);
                        if (s.Read() != '=')
                            throw new CalendarException("Expected '=' <parameter value>.");
                        while (true)
                        {
                            var value = ReadParameterValue(s);
                            content.Parameters.Add(name, value);
                            if (s.Peek() != ',')
                                break;
                            s.Read(); // eat ','
                        }
                        break;
                    case ':':
                        content.Values = ReadValues(s);
                        return content;
                    default:
                        if (c == -1)
                            throw new CalendarException("Expecting a Parameter or Value (';' or ':'), not end of line.");
                        throw new CalendarException(String.Format("Expecting a Parameter or Value (';' or ':'), not '{0}'.", (char) c));
                }
            }
        }

        string ReadName(TextReader r)
        {
            var s = new StringBuilder();
            while (true)
            {
                switch (r.Peek())
                {
                    case ':':
                    case '=':
                    case ';':
                    case -1:
                        return s.ToString();
                }
                s.Append((char)r.Read());
            }
        }

        /// <summary>
        ///   Read all the values.
        /// </summary>
        public string[] ReadValues()
        {
            return ReadValues(reader);
        }

        string[] ReadValues(TextReader r)
        {
            var values = new string[0];
            var s = new StringBuilder();
            while (true)
            {
                var c = r.Read();
                if (c == -1)
                    break;
                switch (c)
                {
                    case '\\':
                        s.Append(ReadEscaped(r));
                        break;
                    case ',':
                        Array.Resize(ref values, values.Length + 1);
                        values[values.Length - 1] = s.ToString();
                        s.Clear();
                        break;
                    default:
                        s.Append((char)c);
                        break;
                }
            }
            Array.Resize(ref values, values.Length + 1);
            values[values.Length - 1] = s.ToString();

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new CalendarException("The <value> cannot be empty."); 
            }
            return values;
        }

        string ReadEscaped(TextReader r)
        {
            char c = (char)r.Read();
            switch (c)
            {
                case 'n':
                case 'N':
                    return "\r\n";
                // Specification says only '\', ';' and ','
                default:
                    return c.ToString();
            }
        }
        string ReadParameterValue(TextReader r)
        {
            if ('"' == r.Peek())
                return ReadQuotedParameterValue(r);

            var s = new StringBuilder();
            while (true)
            {
                switch (r.Peek())
                {
                    case ';':
                    case ':':
                    case ',':
                    case -1:
                        return s.ToString();
                }
                s.Append((char)r.Read());
            }
        }

        string ReadQuotedParameterValue(TextReader r)
        {
            r.Read(); // eat the quote
            var s = new StringBuilder();
            while (true)
            {
                char c = (char)r.Read();
                if (c == '"')
                    break;
                s.Append(c);
            }
            return s.ToString().Replace("\\n", "\n");
        }

    }
}
