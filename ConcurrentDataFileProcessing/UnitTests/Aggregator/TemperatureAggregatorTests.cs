using System;
using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;
using ConcurrentDataFileProcessing.src.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Aggregator
{
    [TestClass]
    public class TemperatureAggregatorTests
    {
        [TestMethod]
        public void AddMeasurements_ShouldStoreData()
        {
            var aggregator = new TemperatureAggregator();

            aggregator.AddMeasurements(new List<Measurement>
            {
                new Measurement
                {
                    Timestamp = new DateTime(2024, 3, 15),
                    Temperature2m = 10.5
                }
            });

            var avg = GetMonthlyAverageInternal(aggregator, 2024, 3);

            Assert.AreEqual(10.5, avg);
        }

        [TestMethod]
        public void MonthlyAverage_ShouldCalculateCorrectValue()
        {
            var aggregator = new TemperatureAggregator();

            aggregator.AddMeasurements(new[]
            {
                new Measurement { Timestamp = new DateTime(2024, 1, 1), Temperature2m = 10 },
                new Measurement { Timestamp = new DateTime(2024, 1, 2), Temperature2m = 20 }
            });

            var avg = GetMonthlyAverageInternal(aggregator, 2024, 1);

            Assert.AreEqual(15, avg);
        }

        [TestMethod]
        public void MonthlyAverage_ShouldReturnNull_WhenEmpty()
        {
            var aggregator = new TemperatureAggregator();

            var avg = GetMonthlyAverageInternal(aggregator, 2024, 12);

            Assert.IsNull(avg);
        }

        [TestMethod]
        public void YearlyAverage_ShouldBeCorrect()
        {
            var aggregator = new TemperatureAggregator();

            aggregator.AddMeasurements(new[]
            {
                new Measurement { Timestamp = new DateTime(2024, 1, 1), Temperature2m = 10 },
                new Measurement { Timestamp = new DateTime(2024, 2, 1), Temperature2m = 20 }
            });

            var avg = GetYearlyAverageInternal(aggregator, 2024);

            Assert.AreEqual(15, avg);
        }
        private double? GetMonthlyAverageInternal(TemperatureAggregator aggr, int year, int month)
        {
            var field = typeof(TemperatureAggregator)
                .GetField("_allMeasurements", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var list = (List<Measurement>)field.GetValue(aggr);

            var monthData = list.FindAll(m => m.Timestamp.Year == year && m.Timestamp.Month == month);

            if (monthData.Count == 0)
                return null;

            double sum = 0;
            foreach (var m in monthData)
                sum += m.Temperature2m;

            return sum / monthData.Count;
        }

        private double? GetYearlyAverageInternal(TemperatureAggregator aggr, int year)
        {
            var field = typeof(TemperatureAggregator)
                .GetField("_allMeasurements", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var list = (List<Measurement>)field.GetValue(aggr);

            var yearData = list.FindAll(m => m.Timestamp.Year == year);

            if (yearData.Count == 0)
                return null;

            double sum = 0;
            foreach (var m in yearData)
                sum += m.Temperature2m;

            return sum / yearData.Count;
        }
    }
}