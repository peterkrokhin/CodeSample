using System;

namespace GPNA.DataFiltration.Application
{
    class FrontDetectFilter : IFilter
    {
        private readonly long _id;
        private readonly bool _negative;
        private readonly bool _positive;
        private double? _prevValue;
        private readonly object _filterLocker = new();

        public FrontDetectFilter(long id, bool negative, bool positive, double? prevValue)
        {
            _id = id;
            _negative = negative;
            _positive = positive;
            _prevValue = prevValue;
        }

        public long GetId() => _id;

        public bool ApplyTo(ParameterValue parameter)
        {
            lock (_filterLocker)
            {
                if (_prevValue is null)
                {
                    throw new Exception($"Отсутствует значение prevValue в конфигурации фильтра с Id={_id}.");
                }

                bool result = (_positive & (_prevValue == 0d) & (parameter.Value == 1d)) |
                    (_negative & (_prevValue == 1d) & (parameter.Value == 0.0d));
                return result;
            }
        }

        public void SaveParameterState(ParameterValue parameter, IFilterStore filterStore)
        {
            lock (_filterLocker)
            {
                filterStore.SavePrevValueInFilterConfig(GetFilterConfig());
                _prevValue = parameter.Value;
            }
        }

        private FilterConfig GetFilterConfig()
        {
            FilterConfig filterConfig = new()
            {
                Id = _id,
                PrevValue = _prevValue?.ToString()
            };
            return filterConfig;
        }
    }
}
