using System;

namespace GPNA.DataFiltration.Application
{
    class MeasurementTimeFilter : IFilter
    {
        private readonly long _id;
        private readonly int _min;
        private readonly int _max;
        private DateTime? _prevTimeStamp;
        private readonly object _filterLocker = new();

        public MeasurementTimeFilter(long id, int min, int max, DateTime? prevTimeStamp)
        {
            _id = id;
            _min = min;
            _max = max;
            _prevTimeStamp = prevTimeStamp;
        }

        public long GetId() => _id;

        public bool ApplyTo(ParameterValue parameter)
        {
            lock (_filterLocker)
            {
                if (parameter.Timestamp is null)
                {
                    throw new Exception("Отсутствует значение Timestamp в фильтруемом параметре.");
                }

                if (_prevTimeStamp is null)
                {
                    throw new Exception($"Отсутствует значение prevTimestamp в конфигурации фильтра с Id={_id}.");
                }

                TimeSpan minDuration = TimeSpan.FromMinutes(_min);
                TimeSpan maxDuration = TimeSpan.FromMinutes(_max);
                TimeSpan currentDuration = parameter.Timestamp.Value - _prevTimeStamp.Value;

                bool result = (currentDuration >= minDuration) & (currentDuration <= maxDuration);
                return result;
            }
        }

        public void SaveParameterState(ParameterValue parameter, IFilterStore filterStore)
        {
            lock (_filterLocker)
            {
                filterStore.SavePrevTimestampInFilterConfig(GetFilterConfig());
                _prevTimeStamp = parameter.Timestamp;
            }
        }

        private FilterConfig GetFilterConfig()
        {
            FilterConfig filterConfig = new()
            {
                Id = _id,
                PrevTimeStamp = _prevTimeStamp
            };
            return filterConfig;
        }
    }
}
