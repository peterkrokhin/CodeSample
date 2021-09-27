using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Application
{
    public class FilterStore : IFilterStore
    {
        private readonly IServiceProvider _services;
        private Dictionary<FilterKey, List<FilterData>> _filterCache = new();
        private readonly object cacheLocker = new();
        public int CasheUpdateIntervalMilliseconds { get; set; } = -5_000;

        public FilterStore(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartCyclicUpdate()
        {
            var allFilterConfigs = await GetAllFilterConfigs();
            var newCache = CreateCache(allFilterConfigs);

            lock (cacheLocker)
            {
                _filterCache = newCache;
            }

            await Task.Delay(CasheUpdateIntervalMilliseconds);
            await StartCyclicUpdate();
        }

        public IEnumerable<FilterData>? GetFilterDataByFilterKey(FilterKey key)
        {
            List<FilterData>? filterDataList = null;
            lock (cacheLocker)
            {
                _filterCache.TryGetValue(key, out filterDataList);
            }
            return filterDataList;
        }

        private async Task<IEnumerable<FilterConfig>> GetAllFilterConfigs()
        {
            using var scope = _services.CreateScope();
            var filterConfigRepo = scope.ServiceProvider.GetRequiredService<IFilterConfigRepo>();
            return await filterConfigRepo.GetAllIncludePool();
        }

        private static Dictionary<FilterKey, List<FilterData>> CreateCache(IEnumerable<FilterConfig> allFilterConfigs)
        {
            Dictionary<FilterKey, List<FilterData>> newCache = new();
            foreach (var fc in allFilterConfigs)
            {
                var key = new FilterKey(fc.FilterPool.SourceTopic, fc.WellId, fc.ParameterId);
                FilterData newFilterData = new(fc.FilterType, fc.FilterDetails);
                if (newCache.ContainsKey(key))
                {
                    newCache[key].Add(newFilterData);
                }
                else
                {
                    newCache.Add(key, new List<FilterData>() { newFilterData });
                }
            }
            return newCache;
        }
    }
}
