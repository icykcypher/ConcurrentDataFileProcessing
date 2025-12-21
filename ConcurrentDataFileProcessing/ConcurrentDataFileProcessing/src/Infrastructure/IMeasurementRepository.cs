using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;

namespace ConcurrentDataFileProcessing.src.Infrastructure
{
    public interface IMeasurementRepository
    {
        void InsertMeasurements(IEnumerable<Measurement> measurements);
    }
}