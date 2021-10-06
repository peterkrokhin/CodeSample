using System;

namespace GPNA.DataFiltration.Application
{
    class MeasurementTimeFilter : IFilter
    {
        private int _min;
        private int _max;
        private DateTime? _prevTimeStamp;

        public MeasurementTimeFilter(int min, int max, DateTime? prevTimeStamp)
        {
            _min = min;
            _max = max;
            _prevTimeStamp = prevTimeStamp;
        }

        public bool ApplyTo(ParameterValue parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}
