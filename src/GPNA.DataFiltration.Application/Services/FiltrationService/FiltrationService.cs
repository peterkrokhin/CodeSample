namespace GPNA.DataFiltration.Application
{
    public class FiltrationService
    {
        //private readonly ConcurrentQueue<ParameterValue> _src = new();
        //private readonly ConcurrentQueue<ParameterValue> _good = new();
        //private readonly ConcurrentQueue<ParameterValue> _bad = new();
        //private readonly IFilterConfigRepo _filters;
        private readonly IFilterStore _filterStore;

        public FiltrationService(IFilterStore filterStore)
        {
            _filterStore = filterStore;
        }

        public bool ApplyFilter(ParameterValue parameter, string sourceTopic)
        {
            bool filterResult = true;
            if (parameter.Value == null)
            {
                return true; // или false
            }

           // var sourceTopic = "temp topic";
            // var filters = _filters.GetBySourceTopicAndWellIdAndParameterId(sourceTopic, parameter.WellId, parameter.ParameterId);

            FilterKey key = new(sourceTopic, parameter.WellId, parameter.ParameterId);
            var filters = _filterStore.GetFilterDataByFilterKey(key);

            if (filters == null)
            {
                return true;
            }
            
            foreach (var filter in filters)
            {
                var function = FilterFunctionFactory.GetFilterFunction(filter);
                if (function != null)
                {
                    filterResult &= function(parameter);
                }
            }

            return filterResult;
        }
    }
}
