namespace NContext.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines a static class for providing String type extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the number of <see cref="String.Format(String, Object[])"/> parameters in the specified text.
        /// </summary>
        /// <param name="text">The text to scan.</param>
        /// <returns>Number of required String.Format parameters.</returns>
        public static Int32 MinimumFormatParametersRequired(this String text)
        {
            Int32 counter = -1;
            foreach (Match match in Regex.Matches(text, @"{(\d+)}+", RegexOptions.IgnoreCase))
            {
                Int32 temp = Int32.Parse(match.Groups[1].ToString());
                if (temp > counter)
                {
                    counter = temp;
                }
            }

            return ++counter;
        }

        /// <summary>
        /// Splits a string into a NameValueCollection, where each "namevalue" is separated by
        /// the "OuterSeparator". The parameter "NameValueSeparator" sets the split between Name and Value.
        /// Example:
        /// String nameValueString = "param1=value1;param2=value2";
        /// NameValueCollection nvOut = nameValueString.ToNameValueCollection(';', '=');
        /// The result is a NameValueCollection where:
        /// key[0] is "param1" and value[0] is "value1"
        /// key[1] is "param2" and value[1] is "value2"
        /// </summary>
        /// <param name="nameValueString">String to process</param>
        /// <param name="OuterSeparator">Separator for each "NameValue"</param>
        /// <param name="NameValueSeparator">Separator for Name/Value splitting</param>
        /// <returns>NameValueCollection.</returns>
        public static NameValueCollection ToNameValueCollection(this String nameValueString, Char OuterSeparator, Char NameValueSeparator)
        {
            NameValueCollection nameValueCollection = null;
            nameValueString = nameValueString.TrimEnd(OuterSeparator);
            if (!String.IsNullOrEmpty(nameValueString))
            {
                String[] arrStrings = nameValueString.TrimEnd(OuterSeparator).Split(OuterSeparator);

                foreach (String nameValuePair in arrStrings)
                {
                    Int32 posSep = nameValuePair.IndexOf(NameValueSeparator);
                    String name = nameValuePair.Substring(0, posSep);
                    String value = nameValuePair.Substring(posSep + 1).Trim(new[] { '"' });
                    if (nameValueCollection == null)
                    {
                        nameValueCollection = new NameValueCollection();
                    }

                    nameValueCollection.Add(name, value);
                }
            }

            return nameValueCollection;
        }

        /// <summary>
        /// Gets the bytes from the string using <see cref="UTF8Encoding"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte array.</returns>
        /// <remarks></remarks>
        public static Byte[] ToBytes<TEncoding>(this String value) where TEncoding : Encoding, new()
        {
            if (value == null) return null;

            try
            {
                var encoding = Activator.CreateInstance<TEncoding>();

                return encoding.GetBytes(value);
            }
            catch (EncoderFallbackException ex)
            {
                throw new ArgumentException("Invalid encoding characters encountered.", ex);
            }
        }

        /// <summary>
        /// Gets the bytes from a hexadecimal string.
        /// </summary>
        /// <param name="hexadecimal">The hexadecimal value.</param>
        /// <returns>Converted byte array.</returns>
        /// <remarks></remarks>
        public static Byte[] ToBytesFromHexadecimal(this String hexadecimal)
        {
            if (hexadecimal == null)
            {
                throw new ArgumentNullException("hexadecimal");
            }

            var stringBuilder = new StringBuilder(hexadecimal.ToUpperInvariant());
            if (stringBuilder[0].Equals('0') && stringBuilder[1].Equals('X'))
            {
                stringBuilder.Remove(0, 2);
            }

            if (stringBuilder.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hexadecimal string.");
            }

            var numArray = new Byte[stringBuilder.Length / 2];
            try
            {
                for (var index = 0; index < numArray.Length; ++index)
                {
                    Int32 startIndex = index * 2;
                    numArray[index] = Convert.ToByte(stringBuilder.ToString(startIndex, 2), 16);
                }
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid hexadecimal string.", ex);
            }

            return numArray;
        }

        /// <summary>
        /// Gets the bytes from a base64 encoded string.
        /// </summary>
        /// <param name="base64EncodedString">The base64 encoded string.</param>
        /// <returns>Byte[].</returns>
        /// <exception cref="System.ArgumentException">Thrown when specified string is an invalid base64 encoding.</exception>
        public static Byte[] ToBytesFromBase64(this String base64EncodedString)
        {
            try
            {
                return Convert.FromBase64String(base64EncodedString);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("String contains invalid base64 encoded string.", ex);
            }
        }
    }
}