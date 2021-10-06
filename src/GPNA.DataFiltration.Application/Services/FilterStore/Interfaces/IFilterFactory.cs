namespace GPNA.DataFiltration.Application
{
    interface IFilterFactory
    {
        IFilter Create(FilterConfig filterConfig);
    }
}
