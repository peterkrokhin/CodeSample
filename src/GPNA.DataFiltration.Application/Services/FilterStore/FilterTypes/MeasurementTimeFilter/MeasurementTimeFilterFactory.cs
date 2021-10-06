using System;
using System.Text.Json;

namespace GPNA.DataFiltration.Application
{
    public class MeasurementTimeFilterFactory : IFilterFactory
    {
        public IFilter Create(FilterConfig filterConfig)
        {
            var details = ParseFilterDetails(filterConfig.FilterDetails);
            return new MeasurementTimeFilter(filterConfig.Id, details.Min, details.Max, filterConfig.PrevTimeStamp);
        }

        private static MeasurementTimeFilterDetails ParseFilterDetails(string filterDatails)
        {
            MeasurementTimeFilterDetails? details;
            try
            {
                details = JsonSerializer.Deserialize<MeasurementTimeFilterDetails>(filterDatails);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании объекта MeasurementTimeFilter из конфигурации.", e);
            }

            if (details == null)
            {
                throw new Exception("Ошибка при создании объекта MeasurementTimeFilter из конфигурации.");
            }

            return details;
        }
    }
}
