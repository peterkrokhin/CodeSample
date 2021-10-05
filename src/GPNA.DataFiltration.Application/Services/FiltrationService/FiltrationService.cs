using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    public class FiltrationService : IFiltrationService
    {
        private readonly IFilterStore _filterStore;
        private readonly ILogger<FiltrationService> _logger;
        private readonly IMessageConsumer _messageConsumer;
        private readonly IMessageProducer _messageProducer;
        private const bool NOT_VALID_PARAMETER_FILTER_RESULT = false;
        private const bool NOT_FOUND_FILTERS_FILTER_RESULT = true;

        public FiltrationService(
            IFilterStore filterStore, 
            ILogger<FiltrationService> logger, 
            IMessageConsumer messageConsumer,
            IMessageProducer messageProducer)
        {
            _filterStore = filterStore;
            _logger = logger;
            _messageConsumer = messageConsumer;
            _messageProducer = messageProducer;
        }

        public bool ApplyFilter(ParameterValue parameter, string sourceTopic)
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

        public void Handle(string sourceTopic, string message)
        {
            try
            {
                var parameter = ConvertMessageToParameterValue(message);
                bool filtrationResult = ApplyFilter(parameter, sourceTopic);

                string targetTopic = filtrationResult ?
                    _filterStore.GetGoodTopicBySourceTopic(sourceTopic) :
                    _filterStore.GetBadTopicBySourceTopic(sourceTopic);

                _messageProducer.SendMessage(targetTopic, message);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Ошибка обработки. Параметр {message} принят, однако не обработан и не отправлен.", e);
            }
        }

        private static ParameterValue ConvertMessageToParameterValue(string message)
        {
            ParameterValue? parameter;
            try
            {
                parameter = JsonSerializer.Deserialize<ParameterValue>(message);
            }
            catch (Exception e)
            {
                throw new Exception($"Параметр {message}. Ошибка при приведении к типу ParameterValue.", e);
            }
            if (parameter is null)
            {
                throw new Exception($"Параметр {message}. Ошибка при приведении к типу ParameterValue.");
            }

            return parameter;
        }
    }
}
