namespace GPNA.DataFiltration.Application
{
    public record FilterKey
    {
        public string SourceTopic { get; init; }
        public long WellId { get; init; }
        public long ParameterId { get; init; }

        public FilterKey(string sourceTopic, long wellId, long parameterId)
        {
            SourceTopic = sourceTopic;
            WellId = wellId;
            ParameterId = parameterId;
        }
    }
}
