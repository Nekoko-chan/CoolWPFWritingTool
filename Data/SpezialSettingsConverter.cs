using System;
using System.ComponentModel;
using ExtensionObjects;

namespace Writer.Data
{
    public class SpezialSettingsConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var s = value as string;
            object convertFrom = s != null ? s.ComplexStyleFromXmlString() : base.ConvertFrom(context, culture, value);
            return convertFrom;
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string xmlString = ((ComplexStyle)value).ToXmlString();
                return xmlString;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ComplexStylesConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var s = value as string;
            object convertFrom = s != null ? s.ComplexStylesFromXmlString() : base.ConvertFrom(context, culture, value);
            return convertFrom;
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string xmlString = ((ComplexStyles)value).ToXmlString();
                return xmlString;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ComplexStyleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var s = value as string;
            object convertFrom = s != null ? s.ComplexStyleFromXmlString() : base.ConvertFrom(context, culture, value);
            return convertFrom;
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string xmlString = ((ComplexStyle)value).ToXmlString();
                return xmlString;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}