using System;

namespace GPNA.DataFiltration.WebApi
{
    public class FilterPoolAddModel
    {
        public long Id { get; set; }
        public string SourceTopic { get; set; }
        public string GoodTopic { get; set; }
        public string BadTopic { get; set; }
        public Guid AccountId { get; set; }
        public bool IsEnabled { get; set; }
    }
}
