using System;
using System.Collections.Generic;
using GPNA.DataFiltration.Application;
using Moq;
using NUnit.Framework;

namespace GPNA.DataFiltration.UnitTests
{
    public class FiltrationServiceUnitTest
    {
        private FiltrationService _filtrationService;
        private Mock<IFilterStore> _mockFilterStore;
        private Dictionary<FilterKey, List<FilterData>> _filterCache;

        [OneTimeSetUp]
        public void Init()
        {
            CreateTestFilterCache();
            _mockFilterStore = new();
            _filtrationService = new(_mockFilterStore.Object);
        }

        [TestCase("ValueRange", 111.11d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("ValueRange", 155.55d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("ValueRange", 333.33d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Positive PrevValueFalse", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Positive PrevValueFalse", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("FrontDetect Positive PrevValueTrue", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Positive PrevValueTrue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Negative PrevValueFalse", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Negative PrevValueFalse", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("FrontDetect Negative PrevValueTrue", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("FrontDetect Negative PrevValueTrue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = false)]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:01:00.000", ExpectedResult = false)]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:15:00.000", ExpectedResult = true)]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:25:00.000", ExpectedResult = false)]
        [TestCase("VoidFilterList", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("NotValidFilterType", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("NotValidFilterDetails", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("NotValidPrevValue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        [TestCase("NotValidPrevTimeStamp", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = true)]
        public bool ApplyFilterTest(string sourceTopic, double value, string timeStamp)
        {
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);
            
            ParameterValue parameterValue = new()
                { WellId = 1, ParameterId = 1, Value = value, Timestamp = DateTime.Parse(timeStamp) };

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }

        private void CreateTestFilterCache()
        {
            _filterCache = new();
            _filterCache
                .Add(
                    new FilterKey("ValueRange", 1, 1),
                    new List<FilterData>() { 
                        new FilterData(10, "ValueRange", "{\"Min\": 111.111, \"Max\": 222.222}", null, null)
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Positive PrevValueFalse", 1, 1),
                    new List<FilterData>() { 
                        new FilterData(10, "FrontDetect", "{\"Positive\": true, \"Negative\": false}", "0.0", null) 
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Positive PrevValueTrue", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": true, \"Negative\": false}", "1.0", null)
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Negative PrevValueFalse", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": false, \"Negative\": true}", "0.0", null)
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Negative PrevValueTrue", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": false, \"Negative\": true}", "1.0", null)
                    });

            _filterCache
                .Add(
                    new FilterKey("MeasurementTime", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "MeasurementTime", "{\"Min\": 10, \"Max\": 20}", null, DateTime.Parse("2021-01-10T00:00:00.000"))
                    });

            _filterCache
                .Add(
                    new FilterKey("VoidFilterList", 1, 1),
                    new List<FilterData>());

            _filterCache
                .Add(
                    new FilterKey("NotValidFilterType", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "NotValid", "{\"Min\": 111.111, \"Max\": 222.222}", null, null)
                    });

            _filterCache
                .Add(
                    new FilterKey("NotValidFilterDetails", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "ValueRange", "{\"NotValid\": 111.111, \"Max\": 222.222}", null, null)
                    });

            _filterCache
                .Add(
                    new FilterKey("NotValidPrevValue", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": false, \"Negative\": true}", "NotValid", null)
                    });

            _filterCache
                .Add(
                    new FilterKey("NotValidPrevTimeStamp", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "MeasurementTime", "{\"Min\": 10, \"Max\": 20}", null, null)
                    });
        }
    }
}