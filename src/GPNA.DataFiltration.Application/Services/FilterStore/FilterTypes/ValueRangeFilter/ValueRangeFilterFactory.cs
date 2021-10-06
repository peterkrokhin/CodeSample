using System;
using System.Text.Json;

namespace GPNA.DataFiltration.Application
{
    public class ValueRangeFilterFactory : IFilterFactory
    {
        public IFilter Create(FilterConfig filterConfig)
        {
            var details = ParseFilterDetails(filterConfig.FilterDetails);
            return new ValueRangeFilter(filterConfig.Id, details.Min, details.Max);
        }

        private static ValueRangeFilterDetails ParseFilterDetails(string filterDatails)
        {
            ValueRangeFilterDetails? details;
            try
            {
                details = JsonSerializer.Deserialize<ValueRangeFilterDetails>(filterDatails);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании объекта ValueRangeFilter из конфигурации.", e);
            }

            if (details == null)
            {
                throw new Exception("Ошибка при создании объекта ValueRangeFilter из конфигурации.");
            }
            return details;
        }
    }
}
