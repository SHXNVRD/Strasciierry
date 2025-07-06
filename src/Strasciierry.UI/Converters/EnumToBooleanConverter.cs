using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Converters;

public abstract class EnumToBooleanConverter<T> : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");

            var enumValue = Enum.Parse(typeof(T), enumString);

            return enumValue.Equals(value);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
            return Enum.Parse(typeof(T), enumString);

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }
}
