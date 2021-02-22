using System;
using System.ComponentModel;

namespace DDrop.Utility.SettingsRepository
{
    public static class StringConvertationHelper
    {
        /// <summary>
        /// Осуществляет конвертацию string -> T
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static T Convert<T>(string inputValue, T defaultValue = default(T))
        {
            if (inputValue == null)
                return defaultValue;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (T)converter.ConvertFromInvariantString(inputValue);
            }

            throw new NotSupportedException(string.Format("There are no supported convertors for {0}", typeof(T).FullName));
        }
    }
}