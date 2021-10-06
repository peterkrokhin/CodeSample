namespace GPNA.DataFiltration.Application
{
    public class ValueRangeFilter : IFilter
    {
        private double _min;
        private double _max;

        public ValueRangeFilter(double min, double max)
        {
            _min = min;
            _max = max;
        }

        public bool ApplyTo(ParameterValue parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}
