using System;
using System.Collections.Generic;

namespace GPNA.DataFiltration.WebApi
{
    public class FilterPoolDto
    {
        public long Id { get; set; }
        public string SourceTopic { get; set; }
        public string GoodTopic { get; set; }
        public string BadTopic { get; set; }
        public Guid AccountId { get; set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<FilterConfigDto> FilterConfigs { get; set; }
    }
}
