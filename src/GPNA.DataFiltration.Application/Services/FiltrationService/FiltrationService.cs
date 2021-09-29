namespace GPNA.DataFiltration.Application
{
    public class FiltrationService
    {
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
                return false;
            }
            if (parameter.Timestamp == null)
            {
                return false;
            }

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
