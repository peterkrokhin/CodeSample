using System;
using System.Text.Json;

namespace GPNA.DataFiltration.Application
{
    public class FilterFunctionFactory
    {
        public const string VALUE_RANGE_FILTER_TYPE = "ValueRange";
        public const string FRONT_DETECT_FILTER_TYPE = "FrontDetect";
        public const string MEASUREMENT_TIME_FILTER_TYPE = "MeasurementTime";

        public static Func<ParameterValue, bool>? GetFilterFunction(FilterData filter)
        {
            Func<ParameterValue, bool>? function = null;

            if (filter.FilterType == VALUE_RANGE_FILTER_TYPE)
            {
                function = GetValueRangeFilterFunction(filter);
            }
            if (filter.FilterType == FRONT_DETECT_FILTER_TYPE)
            {
                function = GetFrontDetectFilterFunction(filter);
            }
            if (filter.FilterType == MEASUREMENT_TIME_FILTER_TYPE)
            {
                function = GetMeasurementTimeFilterFunction(filter);
            }

            return function;
        }

        private static Func<ParameterValue, bool>? GetValueRangeFilterFunction(FilterData filter)
        {
            Func<ParameterValue, bool>? function = null;
            ValueRangeFilterDetails? details = null;

            try
            {
                details = JsonSerializer.Deserialize<ValueRangeFilterDetails>(filter.FilterDetails);
            }
            catch (Exception e)
            {
                return null;
            }

            if (details == null)
            {
                return null;
            }
            if (details.Min == null)
            {
                return null;
            }
            if (details.Max == null)
            {
                return null;
            }

            function = (ParameterValue parameter) =>
                (parameter.Value >= details.Min) & (parameter.Value <= details.Max);

            return function;
        }

        private static Func<ParameterValue, bool>? GetFrontDetectFilterFunction(FilterData filter)
        {
            Func<ParameterValue, bool>? function = null;
            FrontDetectFilterDetails? details = null;
            try
            {
                details = JsonSerializer.Deserialize<FrontDetectFilterDetails>(filter.FilterDetails);
            }
            catch
            {
                return null;
            }
            if (details == null)
            {
                return null;
            }
            if (details.Positive == null)
            {
                return null;
            }
            if (details.Negative == null)
            {
                return null;
            }
            if (details.PrevValue == null)
            {
                return null;
            }

            function = (ParameterValue parameter) =>
            {
                bool result = (details.Positive.Value & !details.PrevValue.Value & (parameter.Value == 1)) |
                    (details.Negative.Value & details.PrevValue.Value & (parameter.Value == 0));
                return result;
            };


            return function;
        }

        private static Func<ParameterValue, bool>? GetMeasurementTimeFilterFunction(FilterData filter)
        {
            Func<ParameterValue, bool>? function = null;
            MeasurementTimeFilterDetails? details = null;

            try
            {
                details = JsonSerializer.Deserialize<MeasurementTimeFilterDetails>(filter.FilterDetails);
            }
            catch
            {
                return null;
            }
            if (details == null)
            {
                return null;
            }
            if (details.Min == null)
            {
                return null;
            }
            if (details.Max == null)
            {
                return null;
            }

            if (details.PrevTimeStamp == null)
            {
                return null;
            }

            function = (ParameterValue parameter) =>
            {
                if (parameter.Timestamp == null)
                {
                    return true;
                }
                TimeSpan minDuration = TimeSpan.FromMinutes(details.Min.Value);
                TimeSpan maxDuration = TimeSpan.FromMinutes(details.Max.Value);
                TimeSpan currentDuration = parameter.Timestamp.Value - details.PrevTimeStamp.Value;
                bool result = (currentDuration >= minDuration) & (currentDuration <= maxDuration);
                return result;
            };

            return function;
        }
    }
}
