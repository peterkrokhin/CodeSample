namespace GPNA.DataFiltration.Application
{
    public interface IFilter
    {
        long GetId();
        bool ApplyTo(ParameterValue parameter);
        void SaveParameterState(ParameterValue parameter, IFilterStore filterStore);
    }
}
