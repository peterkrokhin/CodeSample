using System;
using System.Collections.Generic;
using GPNA.DataFiltration.Application;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GPNA.DataFiltration.UnitTests
{
    public class FiltersApplicatorUnitTest
    {
        private FiltersApplicator _filtersApplicator;
        private Mock<IFilterStore> _mockFilterStore;
        private Mock<ILogger<FiltersApplicator>> _mockLogger;
        private Dictionary<FilterKey, List<IFilter>> _filterCache;

        [OneTimeSetUp]
        public void Init()
        {
            _mockFilterStore = new();
            _mockLogger = new();
            _filtersApplicator = new(_mockFilterStore.Object, _mockLogger.Object);
        }

        [SetUp]
        public void InitTestCase()
        {
            CreateTestFilterCache();
        }

        [TestCase("ValueRange", 100.11d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("ValueRange", 155.55d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        [TestCase("ValueRange", 333.33d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Positive PrevValueFalse", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Positive PrevValueFalse", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        [TestCase("FrontDetect Positive PrevValueTrue", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Positive PrevValueTrue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Negative PrevValueFalse", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Negative PrevValueFalse", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("FrontDetect Negative PrevValueTrue", 0.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        [TestCase("FrontDetect Negative PrevValueTrue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Bad")]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:01:00.000", ExpectedResult = "Bad")]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:15:00.000", ExpectedResult = "Good")]
        [TestCase("MeasurementTime", 1.0d, "2021-01-10T00:25:00.000", ExpectedResult = "Bad")]
        [TestCase("VoidFilterList", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        [TestCase("NotValidPrevValue", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        [TestCase("NotValidPrevTimeStamp", 1.0d, "2021-01-10T00:00:00.000", ExpectedResult = "Good")]
        public string ApplyTest(string sourceTopic, double value, string timeStamp)
        {
            FilterKey key = new(sourceTopic, 1, 1);
            _mockFilterStore.Setup(a => a.GetFilterByFilterKey(key)).Returns(_filterCache[key]);
            _mockFilterStore.Setup(a => a.GetGoodTopicBySourceTopic(sourceTopic)).Returns("Good");
            _mockFilterStore.Setup(a => a.GetBadTopicBySourceTopic(sourceTopic)).Returns("Bad");

            ParameterValue parameterValue = new()
            {
                WellId = 1,
                ParameterId = 1,
                Value = value, 
                Timestamp = DateTime.Parse(timeStamp) 
            };

            var result = _filtersApplicator.Apply(parameterValue, sourceTopic);
            return result;
        }

        private void CreateTestFilterCache()
        {
            _filterCache = new();
            _filterCache
                .Add(
                    new FilterKey("ValueRange", 1, 1),
                    new List<IFilter>()
                    {
                        new ValueRangeFilterFactory().Create(new FilterConfig() 
                        {
                            FilterType = "ValueRange",
                            FilterDetails = "{\"Min\": 111.111, \"Max\": 222.222}"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Positive PrevValueFalse", 1, 1),
                    new List<IFilter>()
                    {
                        new FrontDetectFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "FrontDetect",
                            FilterDetails = "{\"Positive\": true, \"Negative\": false}",
                            PrevValue = "0.0"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Positive PrevValueTrue", 1, 1),
                    new List<IFilter>()
                    {
                        new FrontDetectFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "FrontDetect",
                            FilterDetails = "{\"Positive\": true, \"Negative\": false}",
                            PrevValue = "1.0"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Negative PrevValueFalse", 1, 1),
                    new List<IFilter>() 
                    {
                        new FrontDetectFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "FrontDetect",
                            FilterDetails = "{\"Positive\": false, \"Negative\": true}",
                            PrevValue = "0.0"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("FrontDetect Negative PrevValueTrue", 1, 1),
                    new List<IFilter>() {
                        new FrontDetectFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "FrontDetect",
                            FilterDetails = "{\"Positive\": false, \"Negative\": true}",
                            PrevValue = "1.0"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("MeasurementTime", 1, 1),
                    new List<IFilter>()
                    {
                        new MeasurementTimeFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "MeasurementTime",
                            FilterDetails = "{\"Min\": 10, \"Max\": 20}",
                            PrevTimeStamp = DateTime.Parse("2021-01-10T00:00:00.000")
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("VoidFilterList", 1, 1),
                    new List<IFilter>());

            _filterCache
                .Add(
                    new FilterKey("NotValidPrevValue", 1, 1),
                    new List<IFilter>()
                    {
                        new FrontDetectFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "FrontDetect",
                            FilterDetails = "{\"Positive\": true, \"Negative\": false}",
                            PrevValue = "NotValid"
                        })
                    });

            _filterCache
                .Add(
                    new FilterKey("NotValidPrevTimeStamp", 1, 1),
                    new List<IFilter>()
                    {
                        new MeasurementTimeFilterFactory().Create(new FilterConfig()
                        {
                            FilterType = "MeasurementTime",
                            FilterDetails = "{\"Min\": 10, \"Max\": 20}",
                            PrevTimeStamp = null
                        })
                    });
        }
    }
}
