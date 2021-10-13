using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Application
{
    public class FilterStore : IFilterStore
    {
        private const string VALUE_RANGE_FILTER_TYPE = "ValueRange";
        private const string FRONT_DETECT_FILTER_TYPE = "FrontDetect";
        private const string MEASUREMENT_TIME_FILTER_TYPE = "MeasurementTime";

        private readonly IServiceProvider _services;
        private readonly object cacheLocker = new();
        private Dictionary<FilterKey, List<IFilter>> _filterCache = new();
        private Dictionary<string, (string GoodTopic, string BadTopic)> _poolCache = new();

        public FilterStore(IServiceProvider services)
        {
            _services = services;
        }

        public void CreateCache()
        {
            try
            {
                lock (cacheLocker)
                {
                    var allFilterConfigs = GetAllFilterConfigs();
                    _filterCache = CreateFilterCache(allFilterConfigs);

                    var allFilterPools = GetAllFilterPools();
                    _poolCache = CreatePoolCache(allFilterPools);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при попытке создания кэша фильтров.", e);
            }
        }

        private IEnumerable<FilterConfig> GetAllFilterConfigs()
        {
            using var scope = _services.CreateScope();
            var filterConfigRepo = scope.ServiceProvider.GetRequiredService<IFilterConfigRepo>();
            return filterConfigRepo.GetAllIncludePool();
        }

        private static Dictionary<FilterKey, List<IFilter>> CreateFilterCache(IEnumerable<FilterConfig> allFilterConfigs)
        {
            Dictionary<FilterKey, List<IFilter>> newCache = new();
            foreach (var fc in allFilterConfigs)
            {
                var key = new FilterKey(fc.FilterPool.SourceTopic, fc.WellId, fc.ParameterId);
                var filter = CreateFilter(fc);

                if (newCache.ContainsKey(key))
                {
                    newCache[key].Add(filter);
                }
                else
                {
                    newCache.Add(key, new List<IFilter>() { filter });
                }
            }
            return newCache;
        }

        private static IFilter CreateFilter(FilterConfig filterConfig)
        {
            IFilter filter;
            if (filterConfig.FilterType == VALUE_RANGE_FILTER_TYPE)
            {
                filter = new ValueRangeFilterFactory().Create(filterConfig);
            }
            else if (filterConfig.FilterType == FRONT_DETECT_FILTER_TYPE)
            {
                filter = new FrontDetectFilterFactory().Create(filterConfig);
            }
            else if (filterConfig.FilterType == MEASUREMENT_TIME_FILTER_TYPE)
            {
                filter = new MeasurementTimeFilterFactory().Create(filterConfig);
            }
            else
            {
                throw new Exception("Неизвестный тип фильтра.");
            }
            return filter;
        }

        private IEnumerable<FilterPool> GetAllFilterPools()
        {
            using var scope = _services.CreateScope();
            var filterPoolRepo = scope.ServiceProvider.GetRequiredService<IFilterPoolRepo>();
            return filterPoolRepo.GetAll();
        }

        private static Dictionary<string, (string GoodTopic, string BadTopic)> CreatePoolCache(IEnumerable<FilterPool> allFilterPools)
        {
            Dictionary<string, (string GoodTopic, string BadTopic)> newCache = new();
            foreach (var fc in allFilterPools)
            {
                string sourceTopic = fc.SourceTopic;
                string goodTopic = fc.GoodTopic;
                string badTopic = fc.BadTopic;
                if (!newCache.ContainsKey(sourceTopic))
                {
                    newCache.Add(sourceTopic, (goodTopic, badTopic));
                }
            }
            return newCache;
        }

        public IEnumerable<IFilter>? GetFilterByFilterKey(FilterKey key)
        {
            List<IFilter>? filters;
            lock (cacheLocker)
            {
                _filterCache.TryGetValue(key, out filters);
            }
            return filters;
        }

        public IEnumerable<string> GetSourceTopics()
        {
            var sourceTopics = _poolCache.Keys;
            return sourceTopics;
        }

        public string GetGoodTopicBySourceTopic(string sourceTopic)
        {
            try
            {
                string goodTopic = _poolCache[sourceTopic].GoodTopic;
                return goodTopic;
            }
            catch (Exception e)
            {
                throw new Exception ($"Ошибка при определении топика назначения.", e);
            }
        }

        public string GetBadTopicBySourceTopic(string sourceTopic)
        {
            try
            {
                var goodTopic = _poolCache[sourceTopic].BadTopic;
                return goodTopic;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка при определении топика назначения.", e);
            }
        }

        public void SavePrevTimestampInFilterConfig(FilterConfig filterConfig)
        {
            throw new NotImplementedException();
        }

        public void SavePrevValueInFilterConfig(FilterConfig filterConfig)
        {
            throw new NotImplementedException();
        }
    }
}
