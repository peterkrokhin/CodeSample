using System;

namespace GPNA.DataFiltration.Application
{
    public interface IFiltrationService
    {
        bool ApplyFilter(ParameterValue parameter, string sourceTopic);
    }
}
