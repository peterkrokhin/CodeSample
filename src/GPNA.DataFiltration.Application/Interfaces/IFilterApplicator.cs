namespace GPNA.DataFiltration.Application
{
    interface IFilterApplicator
    {
        string Apply(ParameterValue parameter, string sourceTopic);
    }
}
