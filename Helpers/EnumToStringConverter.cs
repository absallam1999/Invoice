using AutoMapper;
using System;
using System.ComponentModel;

namespace invoice.Helpers
{
    // Converter from string to enum
    public class StringToEnumConverter<TEnum> : IValueConverter<string, TEnum>
        where TEnum : struct, Enum
    {
        private readonly TEnum _defaultValue;
        private readonly bool _useDescriptionAttribute;

        public StringToEnumConverter(TEnum defaultValue = default, bool useDescriptionAttribute = false)
        {
            _defaultValue = defaultValue;
            _useDescriptionAttribute = useDescriptionAttribute;
        }

        public TEnum Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
                return _defaultValue;

            // First try parsing directly
            if (Enum.TryParse<TEnum>(sourceMember, true, out var result))
                return result;

            // If using description attributes, try to match by description
            if (_useDescriptionAttribute)
            {
                foreach (var field in typeof(TEnum).GetFields())
                {
                    if (field.FieldType != typeof(TEnum))
                        continue;

                    var descriptionAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (descriptionAttr.Length > 0)
                    {
                        var description = ((DescriptionAttribute)descriptionAttr[0]).Description;
                        if (string.Equals(description, sourceMember, StringComparison.OrdinalIgnoreCase))
                        {
                            return (TEnum)field.GetValue(null);
                        }
                    }
                }
            }

            // Try to parse by value (numeric string)
            if (int.TryParse(sourceMember, out int numericValue) && Enum.IsDefined(typeof(TEnum), numericValue))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), numericValue);
            }

            return _defaultValue;
        }
    }

    // Converter from enum to string
    public class EnumToStringConverter<TEnum> : IValueConverter<TEnum, string>
        where TEnum : struct, Enum
    {
        private readonly bool _useDescriptionAttribute;

        public EnumToStringConverter(bool useDescriptionAttribute = false)
        {
            _useDescriptionAttribute = useDescriptionAttribute;
        }

        public string Convert(TEnum sourceMember, ResolutionContext context)
        {
            if (_useDescriptionAttribute)
            {
                var field = typeof(TEnum).GetField(sourceMember.ToString());
                if (field != null)
                {
                    var descriptionAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (descriptionAttr.Length > 0)
                    {
                        return ((DescriptionAttribute)descriptionAttr[0]).Description;
                    }
                }
            }

            return sourceMember.ToString();
        }
    }

    // Static helper class for manual conversions
    public static class EnumConverter
    {
        public static string EnumToString<TEnum>(TEnum value, bool useDescription = false) where TEnum : struct, Enum
        {
            var converter = new EnumToStringConverter<TEnum>(useDescription);
            return converter.Convert(value, null);
        }

        public static TEnum StringToEnum<TEnum>(string value, TEnum defaultValue = default, bool useDescription = false) where TEnum : struct, Enum
        {
            var converter = new StringToEnumConverter<TEnum>(defaultValue, useDescription);
            return converter.Convert(value, null);
        }

        public static bool TryStringToEnum<TEnum>(string value, out TEnum result, TEnum defaultValue = default, bool useDescription = false) where TEnum : struct, Enum
        {
            try
            {
                result = StringToEnum(value, defaultValue, useDescription);
                return true;
            }
            catch
            {
                result = defaultValue;
                return false;
            }
        }
    }

    // Extension methods for enums
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes?.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : value.ToString();
        }

        public static TEnum ParseFromDescription<TEnum>(this string description, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            return EnumConverter.StringToEnum(description, defaultValue, useDescription: true);
        }

        public static bool TryParseFromDescription<TEnum>(this string description, out TEnum result, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            return EnumConverter.TryStringToEnum(description, out result, defaultValue, useDescription: true);
        }
    }

    // Attribute for custom enum display names
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public string Name { get; }

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}