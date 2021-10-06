namespace GPNA.DataFiltration.Application
{
    public interface IFilter
    {
        bool ApplyTo(ParameterValue parameter);
    }
}
