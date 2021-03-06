using System;

namespace GPNA.DataFiltration.WebApi
{
    public class FilterConfigDto
    {
        public long Id { get; set; }
        public long FilterPoolId { get; set; }
        public long WellId { get; set; }
        public int ParameterId { get; set; }
        public string FilterType { get; set; }
        public string FilterDetails { get; set; }
        public bool IsEnabled { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatetAt { get; set; }
        public FilterPoolDto FilterPool { get; set; }
    }
}
