using System.Collections.Generic;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterStore
    {
        void CreateCache();
        public IEnumerable<IFilter>? GetFilterByFilterKey(FilterKey key);
        IEnumerable<string> GetSourceTopics();
        string GetGoodTopicBySourceTopic(string sourceTopic);
        string GetBadTopicBySourceTopic(string sourceTopic);
        void SavePrevTimestampInFilterConfig(FilterConfig filterConfig);
        void SavePrevValueInFilterConfig(FilterConfig filterConfig);
    }
}
