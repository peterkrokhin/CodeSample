using System;

namespace GPNA.DataFiltration.Application
{
    public record FilterData(
        long Id, 
        string FilterType, 
        string FilterDetails, 
        string PrevValue,
        DateTime? PrevTimeStamp);
}
