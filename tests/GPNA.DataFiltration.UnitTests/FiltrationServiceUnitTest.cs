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

        [SetUp]
        public void Setup()
        {
            CreateTestFilterCache();
            _mockFilterStore = new();
            _filtrationService = new(_mockFilterStore.Object);
        }

        [TestCase(100.11d, ExpectedResult = false)]
        [TestCase(150.55d, ExpectedResult = true)]
        [TestCase(300.33d, ExpectedResult = false)]
        public bool ValueRangeFilterTest(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };

            string sourceTopic = "TestTopic ValueRange";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);

            return result;
        }

        [TestCase(0.0d, ExpectedResult = false)]
        [TestCase(1.0d, ExpectedResult = true)]
        public bool PositivePrevValueFalseFrontDetectFilterTest(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };

            string sourceTopic = "TestTopic Positive PrevValueFalse FrontDetect";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }

        [TestCase(0.0d, ExpectedResult = false)]
        [TestCase(1.0d, ExpectedResult = false)]
        public bool PositivePrevValueTrueFrontDetectFilterTest(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };

            string sourceTopic = "TestTopic Positive PrevValueTrue FrontDetect";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }

        [TestCase(0.0d, ExpectedResult = false)]
        [TestCase(1.0d, ExpectedResult = false)]
        public bool NegativePrevValueFalseFrontDetectFilterTest(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };

            string sourceTopic = "TestTopic Negative PrevValueFalse FrontDetect";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }

        [TestCase(0.0d, ExpectedResult = true)]
        [TestCase(1.0d, ExpectedResult = false)]
        public bool NegativePrevValueTrueFrontDetectFilterTest(double value)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = value };

            string sourceTopic = "TestTopic Negative PrevValueTrue FrontDetect";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }

        [TestCase("2021-01-10T00:01:00.000", ExpectedResult = false)]
        [TestCase("2021-01-10T00:15:00.000", ExpectedResult = true)]
        [TestCase("2021-01-10T00:25:00.000", ExpectedResult = false)]
        public bool MeasurementTimeFilterTest(string timestamp)
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = 1.0d, Timestamp = DateTime.Parse(timestamp) };

            string sourceTopic = "TestTopic MeasurementTime";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            return result;
        }
        
        [Test]
        public void NullValueInParameterFilterTest()
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = null };

            string sourceTopic = "TestTopic ValueRange";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            Assert.That(!result);
        }

        [Test]
        public void NullTimeStampInParameterFilterTest()
        {
            ParameterValue parameterValue = new() { WellId = 1, ParameterId = 1, Value = 1.0d, Timestamp = null };

            string sourceTopic = "TestTopic ValueRange";
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterDataByFilterKey(key)).Returns(_filterCache[key]);

            var result = _filtrationService.ApplyFilter(parameterValue, sourceTopic);
            Assert.That(!result);
        }

        [Test]
        public void FiltersNonFoundFilterTest()
        {
            Assert.Pass();
        }

        [Test]
        public void FiltersVoidListFilterTest()
        {
            Assert.Pass();
        }

        [Test]
        public void NotValidFilterTypeFilterTest()
        {
            Assert.Pass();
        }

        [Test]
        public void NotValidFilterDetailsFilterTest()
        {
            Assert.Pass();
        }

        private void CreateTestFilterCache()
        {
            _filterCache = new();
            _filterCache
                .Add(
                    new FilterKey("TestTopic ValueRange", 1, 1),
                    new List<FilterData>() { 
                        new FilterData(10, "ValueRange", "{\"Min\": 111.111, \"Max\": 222.222}")
                    });

            _filterCache
                .Add(
                    new FilterKey("TestTopic Positive PrevValueFalse FrontDetect", 1, 1),
                    new List<FilterData>() { 
                        new FilterData(10, "FrontDetect", "{\"Positive\": true, \"Negative\": false, \"PrevValue\": false}") 
                    });

            _filterCache
                .Add(
                    new FilterKey("TestTopic Positive PrevValueTrue FrontDetect", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": true, \"Negative\": false, \"PrevValue\": true}")
                    });

            _filterCache
                .Add(
                    new FilterKey("TestTopic Negative PrevValueFalse FrontDetect", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": false, \"Negative\": true, \"PrevValue\": false}")
                    });

            _filterCache
                .Add(
                    new FilterKey("TestTopic Negative PrevValueTrue FrontDetect", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "FrontDetect", "{\"Positive\": false, \"Negative\": true, \"PrevValue\": true}")
                    });

            _filterCache
                .Add(
                    new FilterKey("TestTopic MeasurementTime", 1, 1),
                    new List<FilterData>() {
                        new FilterData(10, "MeasurementTime", "{\"Min\": 10, \"Max\": 20, \"PrevTimeStamp\": \"2021-01-10T00:00:00.000\"}")
                    });
        }
    }
}