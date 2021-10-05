using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    class MessageHandler : IMessageHandler
    {
        private readonly IFilterApplicator _filterApplicator;
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(
            IFilterApplicator filterApplicator, 
            IMessageProducer messageProducer, 
            ILogger<MessageHandler> logger)
        {
            _filterApplicator = filterApplicator;
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public void Handle(string sourceTopic, string message)
        {
            try
            {
                var parameter = ConvertMessageToParameterValue(message);
                string targetTopic = _filterApplicator.Apply(parameter, sourceTopic);
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
