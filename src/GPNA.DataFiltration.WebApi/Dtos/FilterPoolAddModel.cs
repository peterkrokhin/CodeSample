using System;
using System.ComponentModel.DataAnnotations;

namespace GPNA.DataFiltration.WebApi
{
    public class FilterPoolAddModel
    {
        [Required]
        public string? SourceTopic { get; set; }
        [Required]
        public string? GoodTopic { get; set; }
        [Required]
        public string? BadTopic { get; set; }
        [Required]
        public Guid? AccountId { get; set; }
        [Required]
        public bool? IsEnabled { get; set; }
    }
}
