using System;
using System.Collections.Generic;
using System.Globalization;
using Izenda.BI.Framework.Constants;
using Izenda.BI.Framework.Models;

namespace CustomAdhocReports
{
    /// <summary>
    /// The custom data format class.
    /// </summary>
    static class CustomDataFormat
    {
        /// <summary>
        /// Loads the custom data formats.
        /// </summary>
        /// <returns>Returns a list of custom data formats.</returns>
        public static List<DataFormat> LoadCustomDataFormat()
        {
            var result = new List<DataFormat>
            {
                new DataFormat
                {
                    Name = "By Hour",
                    DataType = DataType.DateTime,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var date = Convert.ToDateTime(x);
                        return date.ToString("M/d/yyyy h:00 tt");
                    }
                },
                new DataFormat
                {
                    Name = "dd MM:mm",
                    DataType = DataType.DateTime,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var date = Convert.ToDateTime(x);
                        return date.ToString("dd HH:mm");
                    }
                },
                new DataFormat
                {
                    Name = "dd HH:mm:ss",
                    DataType = DataType.DateTime,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var date = Convert.ToDateTime(x);
                        return date.ToString("dd HH:mm:ss");
                    }
                },
                new DataFormat
                {
                    Name = "dd mm:ss",
                    DataType = DataType.DateTime,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var date = Convert.ToDateTime(x);
                        return date.ToString("dd mm:ss");
                    }
                },
                new DataFormat
                {
                    Name = "£0,000",
                    DataType = DataType.Numeric,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var number = Convert.ToDecimal(x);
                        return number.ToString("C0", CultureInfo.CreateSpecificCulture("en-GB"));
                    }
                },
                new DataFormat
                {
                    Name = "¥0,000",
                    DataType = DataType.Numeric,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var number = Convert.ToDecimal(x);
                        return number.ToString("C0", CultureInfo.CreateSpecificCulture("ja-JP"));
                    }
                },
                new DataFormat
                {
                    Name = "0,000",
                    DataType = DataType.Numeric,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        return String.Format(CultureInfo.InvariantCulture, "{0:0,0}", x);
                    }
                },
                new DataFormat
                {
                    Name = "$0,000",
                    DataType = DataType.Numeric,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        return String.Format(CultureInfo.InvariantCulture, "${0:0,0}", x);
                    }
                },
                new DataFormat
                {
                    Name = "HH:MM:SS",
                    DataType = DataType.Numeric,
                    Category = IzendaKey.CustomFormat,
                    FormatFunc = (x) =>
                    {
                        var newValue = Convert.ToDouble(x);
                        TimeSpan time = TimeSpan.FromSeconds(newValue);

                        return time.ToString(@"dd\.hh\:mm\:ss");
                    }
                }
            };

            return result;
        }
    }
}
