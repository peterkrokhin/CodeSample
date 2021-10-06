using System;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    public class FiltersApplicator : IFiltersApplicator
    {
        private const bool NOT_VALID_PARAMETER_FILTER_RESULT = false;
        private const bool NOT_FOUND_FILTERS_FILTER_RESULT = true;
        private const bool NOT_VALID_FILTER_FILTER_RESULT = true;
        private readonly IFilterStore _filterStore;
        private readonly ILogger<FiltersApplicator> _logger;

        public FiltersApplicator(IFilterStore filterStore, ILogger<FiltersApplicator> logger)
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
            var filters = _filterStore.GetFilterByFilterKey(key);

            if (filters is null)
            {
                return NOT_FOUND_FILTERS_FILTER_RESULT;
            }

            bool filterResult = true;
            foreach (var filter in filters)
            {
                try
                {
                    filterResult &= filter.ApplyTo(parameter);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Невозможно применить фильтр с Id={filter.GetId()}. Фильтр пропущен.");
                    filterResult &= NOT_VALID_FILTER_FILTER_RESULT;
                }

                try
                {
                    filter.SaveParameterState(parameter, _filterStore);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Не удалось сохранить состояние параметра для фильтра с Id={filter.GetId()}. Следующая обработка фильтра может быть некорректной.");
                }
            }

            return filterResult;
        }
    }
}
