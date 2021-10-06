namespace GPNA.DataFiltration.Application
{
    public class ValueRangeFilter : IFilter
    {
        private readonly long _id;
        private readonly double _min;
        private readonly double _max;

        public ValueRangeFilter(long id, double min, double max)
        {
            _id = id;
            _min = min;
            _max = max;
        }

        public long GetId() => _id;

        public bool ApplyTo(ParameterValue parameter)
        {
            bool result = (parameter.Value >= _min) & (parameter.Value <= _max);
            return result;
        }

        public void SaveParameterState(ParameterValue parameter, IFilterStore filterStore)
        {
            // В данном типе фильтра ничего не сохраняем.
        }
    }
}
