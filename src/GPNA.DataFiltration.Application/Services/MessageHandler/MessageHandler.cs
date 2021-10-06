using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    class MessageHandler : IMessageHandler
    {
        private readonly IFiltersApplicator _filtersApplicator;
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(
            IFiltersApplicator filtersApplicator, 
            IMessageProducer messageProducer, 
            ILogger<MessageHandler> logger)
        {
            _filtersApplicator = filtersApplicator;
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public void Handle(string sourceTopic, string message)
        {
            try
            {
                var parameter = ConvertMessageToParameterValue(message);
                string targetTopic = _filtersApplicator.Apply(parameter, sourceTopic);
                _messageProducer.SendMessage(targetTopic, message);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Ошибка обработки. Параметр {message} принят, однако не обработан и не отправлен.");
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
