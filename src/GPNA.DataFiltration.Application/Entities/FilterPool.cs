using System;
using System.Collections.Generic;

namespace GPNA.DataFiltration.App
{
    public class FilterPool
    {
        public long Id { get; set; }
        public string SourceTopic { get; set; }
        public string GoodTopic { get; set; }
        public string BadTopic { get; set; }
        public Guid AccountId { get; set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<FilterConfig> FilterConfigs { get; set; }
    }
}
