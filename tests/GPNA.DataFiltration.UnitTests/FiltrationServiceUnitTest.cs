using NUnit.Framework;
using GPNA.DataFiltration.Application;
using Moq;
using System.Collections.Generic;

namespace GPNA.DataFiltration.UnitTests
{
    public class FiltrationServiceUnitTest
    {
        private FiltrationService _filtrationService;
        private Mock<IFilterStore> _mockFilterStore;
        private Dictionary<FilterKey, List<FilterData>> _filterCache;

        [SetUp]
        public void Setup()
        {
            _filterCache = new();
            FilterKey key = new("TestTopic", 1, 1);
            List<FilterData> filters = new();
            filters.Add(new(10, FilterFunctionFactory.VALUE_RANGE_FILTER_TYPE, "{\"Min\": 111.111, \"Max\": 222.222}"));
            _filterCache.Add(key, filters);

            _mockFilterStore = new();
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);
            
            _filtrationService = new(_mockFilterStore.Object);
        }

        [TestCase(150.55)]
        public void Test1(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };
            
            var result = _filtrationService.ApplyFilter(parameterValue, "TestTopic");

            Assert.That(result == true);
        }
    }
}