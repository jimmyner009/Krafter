using Humanizer;

namespace Krafter.UI.Web.Client.Enums;

public class EnumDropDown<T> where T : Enum
{
    public T Value { get; set; }
    public string Name { get; set; }
}

public static class EnumExtensions
{
    public static List<EnumDropDown<T>> ToList<T>() where T : Enum
    {
        var dropDownList = new List<EnumDropDown<T>>();
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            dropDownList.Add(new EnumDropDown<T> { Value = value, Name = value.Humanize() });
        }
        return dropDownList;
    }
}