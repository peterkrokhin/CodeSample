using System;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    public class FilterApplicator : IFilterApplicator
    {
        private readonly IFilterStore _filterStore;
        private readonly ILogger<FilterApplicator> _logger;
        private const bool NOT_VALID_PARAMETER_FILTER_RESULT = false;
        private const bool NOT_FOUND_FILTERS_FILTER_RESULT = true;

        public FilterApplicator(IFilterStore filterStore, ILogger<FilterApplicator> logger)
        {
            _filterStore = filterStore;
            _logger = logger;
        }

        public string Apply(ParameterValue parameter, string sourceTopic)
        {
            string targetTopic;
            bool filterResult = GetFilterResultAndSaveState(parameter, sourceTopic);
            if (filterResult)
            {
                targetTopic = _filterStore.GetGoodTopicBySourceTopic(sourceTopic);
            }
            else
            {
                targetTopic = _filterStore.GetBadTopicBySourceTopic(sourceTopic);
            }
            return targetTopic;
        }

        public bool GetFilterResultAndSaveState(ParameterValue parameter, string sourceTopic)
        {
            if (parameter.Value is null || parameter.Timestamp is null)
            {
                return NOT_VALID_PARAMETER_FILTER_RESULT;
            }

            FilterKey key = new(sourceTopic, parameter.WellId, parameter.ParameterId);
            var filters = _filterStore.GetFilterDataByFilterKey(key);

            if (filters is null)
            {
                return NOT_FOUND_FILTERS_FILTER_RESULT;
            }

            bool filterResult = true;
            foreach (var filter in filters)
            {
                var function = FilterFunctionFactory.GetFilterFunction(filter);

                try
                {
                    SaveParameterValueAndTimeStamp(key, filter, parameter);
                }
                catch (Exception e)
                {
                    string msg = $"Ошибка при сохранении состояния параметра для фильтра с Id={filter.Id}. " +
                        "Следующая сработка фильтра может быть некорректной.";
                    _logger.LogWarning(e, msg);
                }

                if (function is not null)
                {
                    filterResult &= function(parameter);
                }
                else
                {
                    _logger.LogWarning($"Не удалось применить фильтр с Id={filter.Id}. Проверьте конфигурацию фильтра.");
                }
            }

            return filterResult;
        }

        private void SaveParameterValueAndTimeStamp(FilterKey key, FilterData filter, ParameterValue parameter)
        {
            FilterData newFilter = new(
                filter.Id,
                filter.FilterType,
                filter.FilterDetails,
                parameter.Value?.ToString(),
                parameter.Timestamp);

            _filterStore.ModifyFilterDataByFilterKey(key, newFilter);
        }
    }
}
