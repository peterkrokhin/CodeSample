using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    public class FiltrationService : IFiltrationService
    {
        private readonly IFilterStore _filterStore;
        private readonly IMessageConsumer _messageConsumer;
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<FiltrationService> _logger;
        private readonly object _startStopLocker = new();
        private CancellationTokenSource? _cts;
        private List<Task>? _allTasks;

        public FiltrationService(
            IFilterStore filterStore,
            IMessageConsumer messageConsumer,
            IMessageHandler messageHandler,
            ILogger<FiltrationService> logger)
        {
            _filterStore = filterStore;
            _messageConsumer = messageConsumer;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public void Start()
        {
            try
            {
                lock (_startStopLocker)
                {
                    if (_cts is null)
                    {
                        _cts = new CancellationTokenSource();
                    }
                    else
                    {
                        // Фильтрация уже запущена.
                        return;
                    }

                    _filterStore.CreateCache();

                    _allTasks = new();

                    var topics = _filterStore.GetSourceTopics();
                    foreach (var topic in topics)
                    {
                        Task filtrationTask = Task.Run(() =>
                            RunFiltrationTask(topic, _messageHandler, _cts.Token),
                            _cts.Token);

                        _allTasks.Add(filtrationTask);
                    }
                }
            }
            catch
            {
                // Останавливаем при неудачном пуске.
                try
                {
                    Stop();
                }
                catch (Exception e)
                {
                    // Неудачный останов только логгируем.
                    _logger.LogWarning(e, "Ошибка при попытке останова FiltrationService.");
                }

                // Исключение по неудачному пуску пробрасываем дальше.
                throw;
            }
        }

        public void Stop()
        {
            lock (_startStopLocker)
            {
                if (_cts is null)
                {
                    // Фильтрация уже остановлена.
                    return;
                }

                if (_allTasks is null)
                {
                    // Задач не было.
                    _cts.Dispose();
                    _cts = null;
                    return;
                }

                try
                {
                    _cts.Cancel();

                    bool allTasksComleted = false;

                    // Ожидаем завершения всех задач.
                    while (!allTasksComleted)
                    {
                        allTasksComleted = true;
                        foreach (var task in _allTasks)
                        {
                            allTasksComleted &= task.IsCompleted;
                        }
                    }
                }
                finally
                {
                    _cts.Dispose();
                    _cts = null;
                    _allTasks = null;
                }
            }
        }

        private void RunFiltrationTask(
            string topic, 
            IMessageHandler messageHandler, 
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Задача по обработке топика {topic} запущена.");
                _messageConsumer.SubscribeOnTopic(topic, messageHandler, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Задача по обработке топика {topic} остановлена.");
                // Для корректного формирования признака isCanceled
                throw;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Ошибка в задаче по обработке топика {topic}. Задача остановлена.");
            }
        }
    }
}
