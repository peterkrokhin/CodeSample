using System;
using System.Collections.Generic;

namespace GPNA.DataFiltration.Application
{
    public class FilterPool
    {
        public long Id { get; set; }
        public string SourceTopic { get; set; } = String.Empty;
        public string GoodTopic { get; set; } = String.Empty;
        public string BadTopic { get; set; } = String.Empty;
        public Guid AccountId { get; set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<FilterConfig> FilterConfigs { get; set; } = null!;
    }
}
