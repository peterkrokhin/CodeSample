using System;
using System.Globalization;
using System.Text.Json;

namespace GPNA.DataFiltration.Application
{
    public class FilterFunctionFactory
    {
        private const string VALUE_RANGE_FILTER_TYPE = "ValueRange";
        private const string FRONT_DETECT_FILTER_TYPE = "FrontDetect";
        private const string MEASUREMENT_TIME_FILTER_TYPE = "MeasurementTime";

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
            catch
            {
                return null;
            }

            if (details == null)
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
            double prevValue;
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

            try
            {
                prevValue = Convert
                    .ToDouble(filter.PrevValue, new NumberFormatInfo { NumberDecimalSeparator = "." });
            }
            catch
            {
                return null;
            }

            function = (ParameterValue parameter) =>
            {
                bool result = (details.Positive & (prevValue == 0d) & (parameter.Value == 1d)) |
                    (details.Negative & (prevValue == 1d) & (parameter.Value == 0.0d));
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

            if (filter.PrevTimeStamp == null)
            {
                return null;
            }

            function = (ParameterValue parameter) =>
            {
                if (parameter.Timestamp == null)
                {
                    return true;
                }
                TimeSpan minDuration = TimeSpan.FromMinutes(details.Min);
                TimeSpan maxDuration = TimeSpan.FromMinutes(details.Max);
                TimeSpan currentDuration = parameter.Timestamp.Value - filter.PrevTimeStamp.Value;
                bool result = (currentDuration >= minDuration) & (currentDuration <= maxDuration);
                return result;
            };

            return function;
        }
    }
}
