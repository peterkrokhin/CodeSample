namespace GPNA.DataFiltration.Application
{
    class FrontDetectFilter : IFilter
    {
        private bool _negative;
        private bool _positive;
        private double? _prevValue;

        public FrontDetectFilter(bool negative, bool positive, double? prevValue)
        {
            _negative = negative;
            _positive = positive;
            _prevValue = prevValue;
        }

        public bool ApplyTo(ParameterValue parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}
