namespace GPNA.DataFiltration.Application
{
    interface IFiltersApplicator
    {
        string Apply(ParameterValue parameter, string sourceTopic);
    }
}
