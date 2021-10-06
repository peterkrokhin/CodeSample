using System;
using System.Globalization;
using System.Text.Json;

namespace GPNA.DataFiltration.Application
{
    public class FrontDetectFilterFactory : IFilterFactory
    {
        public IFilter Create(FilterConfig filterConfig)
        {
            var details = ParseFilterDetails(filterConfig.FilterDetails);
            var prevValue = ParsePrevValue(filterConfig.PrevValue);
            return new FrontDetectFilter(filterConfig.Id, details.Negative, details.Positive, prevValue);
        }

        private static FrontDetectFilterDetails ParseFilterDetails(string filterDatails)
        {
            FrontDetectFilterDetails? details;
            try
            {
                details = JsonSerializer.Deserialize<FrontDetectFilterDetails>(filterDatails);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании объекта FrontDetectFilter из конфигурации.", e);
            }

            if (details == null)
            {
                throw new Exception("Ошибка при создании объекта FrontDetectFilter из конфигурации.");
            }

            return details;
        }

        private static double? ParsePrevValue(string? parseValue)
        {
            double? value;
            try
            {
                value = Convert
                    .ToDouble(parseValue, new NumberFormatInfo { NumberDecimalSeparator = "." });
            }
            catch
            {
                value = null;
            }
            return value;
        }
    }
}
