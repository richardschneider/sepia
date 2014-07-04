using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   The required properties and methods of a object that is used with calendaring and scheduling.
    /// </summary>
    /// <remarks>
    ///   A calendar component supports the iCalendar data format. It can be serialised with 
    ///   the <see cref="IcsSerializable.ReadIcs"/> and <see cref="IcsSerializable.WriteIcs"/> methods.
    /// </remarks>
    public interface ICalenderComponent : IcsSerializable
    {
    }
}
