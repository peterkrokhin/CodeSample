using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Application
{
    public class FilterStore : IFilterStore
    {
        private readonly IServiceProvider _services;
        private readonly object cacheLocker = new();
        private Dictionary<FilterKey, List<FilterData>> _filterCache = new();
        private List<FilterPool> _poolCache = new();

        public FilterStore(IServiceProvider services)
        {
            _services = services;
        }

        public void CacheUpdate()
        {
            try
            {
                lock (cacheLocker)
                {
                    var allFilterConfigs = GetAllFilterConfigs();
                    _filterCache = CreateFilterCache(allFilterConfigs);
                    _poolCache = CreatePoolCache(allFilterConfigs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при попытке обновления кэша конфигураций фильтров.", e);
            }
        }

        private IEnumerable<FilterConfig> GetAllFilterConfigs()
        {
            using var scope = _services.CreateScope();
            var filterConfigRepo = scope.ServiceProvider.GetRequiredService<IFilterConfigRepo>();
            return filterConfigRepo.GetAllIncludePool();
        }

        private static Dictionary<FilterKey, List<FilterData>> CreateFilterCache(IEnumerable<FilterConfig> allFilterConfigs)
        {
            Dictionary<FilterKey, List<FilterData>> newCache = new();
            foreach (var fc in allFilterConfigs)
            {
                var key = new FilterKey(fc.FilterPool.SourceTopic, fc.WellId, fc.ParameterId);
                FilterData newFilterData = new(
                    fc.Id, 
                    fc.FilterType, 
                    fc.FilterDetails, 
                    fc.PrevValue,
                    fc.PrevTimeStamp);

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

        private static List<FilterPool> CreatePoolCache(IEnumerable<FilterConfig> allFilterConfigs)
        {
            List<FilterPool> newCache = new();
            foreach (var filterConfig in allFilterConfigs)
            {
                newCache.Add(filterConfig.FilterPool);
            }
            return newCache;
        }

        public IEnumerable<FilterData>? GetFilterDataByFilterKey(FilterKey key)
        {
            List<FilterData>? filterDataList;
            lock (cacheLocker)
            {
                _filterCache.TryGetValue(key, out filterDataList);
            }
            return filterDataList;
        }

        public void ModifyFilterDataByFilterKey(FilterKey key, FilterData newFilterData)
        {
            try
            {
                lock (cacheLocker)
                {
                    using var scope = _services.CreateScope();
                    var filterConfigRepo = scope.ServiceProvider.GetRequiredService<IFilterConfigRepo>();

                    // Ищем в репозитории.
                    var filterConfig = filterConfigRepo.GetById(newFilterData.Id);
                    if (filterConfig is null)
                    {
                        throw new Exception($"Не найдена конфигурация фильтра с Id={newFilterData.Id} в репозитории.");
                    }

                    // Ищем в кэше.
                    _filterCache.TryGetValue(key, out List<FilterData>? listFilterData);
                    if (listFilterData is null)
                    {
                        throw new Exception($"Неверный ключ при поиске конфигурации фильтра с Id={newFilterData.Id} в кэше.");
                    }
                    var filterData = listFilterData.Find(fd => fd.Id == newFilterData.Id);
                    if (filterData is null)
                    {
                        throw new Exception($"Не найдена конфигурация фильтра с Id={newFilterData.Id} в кэше.");
                    }

                    // Все Ок? Сохраняем в репозитории.
                    filterConfig.FilterDetails = newFilterData.FilterDetails;
                    filterConfigRepo.Update(filterConfig);

                    // Все Ок? Сохраняем в кэше.
                    filterData = newFilterData;
                }
            }
            catch (Exception e)
            {
                string message = $"Ошибка при попытке изменения конфигурации фильтра с Id={newFilterData.Id}.";
                throw new Exception(message, e);
            }
        }

        public IEnumerable<string> GetSourceTopics()
        {
            var sourceTopics = _poolCache.Select(p => p.SourceTopic);
            return sourceTopics;
        }

        public IEnumerable<string> GetGoodTopics()
        {
            var goodTopics = _poolCache.Select(p => p.GoodTopic);
            return goodTopics;
        }

        public string GetGoodTopicBySourceTopic(string sourceTopic)
        {
            try
            {
                var goodTopics = _poolCache
                    .Where(p => p.SourceTopic == sourceTopic)
                    .Select(p => p.GoodTopic)
                    .First();
                return goodTopics;
            }
            catch (Exception e)
            {
                throw new Exception ($"Ошибка при определении топика назначения.", e);
            }
        }

        public IEnumerable<string> GetBadTopics()
        {
            var badTopics = _poolCache.Select(p => p.GoodTopic);
            return badTopics;
        }

        public string GetBadTopicBySourceTopic(string sourceTopic)
        {
            try
            {
                var goodTopics = _poolCache
                    .Where(p => p.SourceTopic == sourceTopic)
                    .Select(p => p.BadTopic)
                    .First();
                return goodTopics;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка при определении топика назначения.", e);
            }
        }
    }
}
