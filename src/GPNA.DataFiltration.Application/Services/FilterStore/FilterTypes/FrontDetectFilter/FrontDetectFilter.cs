using System;

namespace GPNA.DataFiltration.Application
{
    class FrontDetectFilter : IFilter
    {
        private readonly long _id;
        private readonly bool _negative;
        private readonly bool _positive;
        private double? _prevValue;

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
            if (_prevValue is null)
            {
                throw new Exception($"Отсутствует значение prevValue в конфигурации фильтра с Id={_id}.");
            }

            bool result = (_positive & (_prevValue == 0d) & (parameter.Value == 1d)) |
                (_negative & (_prevValue == 1d) & (parameter.Value == 0.0d));
            return result;
        }

        public void SaveParameterState(ParameterValue parameter, IFilterStore filterStore)
        {
            _prevValue = parameter.Value;
            filterStore.SavePrevValueInFilterConfig(GetFilterConfig());
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
