namespace GPNA.DataFiltration.Application
{
    public record FilterData
    {
        public string FilterType { get; init; }
        public string FilterDetails { get; init; }

        public FilterData(string filterType, string filterDetails)
        {
            FilterType = filterType;
            FilterDetails = filterDetails;
        }
    }
}
