using Dapper;
using System.Linq;
using System.Data.SQLite;
using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;

namespace ConcurrentDataFileProcessing.src.Infrastructure
{
    /// <summary>
    /// Class represents sql abstraction methods to initialize the SQLite database, insert measurements, and query data.
    /// </summary>
    public class SqliteRepository
    {
        private readonly string _dbPath;
        private readonly string _connString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteRepository"/> class.
        /// </summary>
        /// <param name="dbPath">The path to the SQLite database file.</param>
        public SqliteRepository(string dbPath)
        {
            _dbPath = dbPath;
            _connString = "Data Source=" + _dbPath + ";Version=3;";
        }


        /// <summary>
        /// Creates the Measurements table if it does not exist.
        /// </summary>
        public void Init()
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                var sql = @"
                CREATE TABLE IF NOT EXISTS Measurements (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    Sensor TEXT NOT NULL,
                    Value REAL NOT NULL
                );";
                conn.Execute(sql);
            }
        }

        /// <summary>
        /// Inserts a collection of <see cref="Measurement"/> objects into the database.
        /// </summary>
        /// <param name="measurements">The measurements to insert.</param>
        public void InsertMeasurements(IEnumerable<Measurement> measurements)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    const string insert =
                        "INSERT INTO Measurements (Timestamp, Sensor, Value) VALUES (@Timestamp, @Sensor, @Value);";

                    var rows = measurements.Select(m => new
                    {
                        Timestamp = m.Timestamp.ToString("o"),
                        m.Sensor,
                        m.Value
                    });

                    conn.Execute(insert, rows, trans);

                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// Queries all measurements from the database.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Measurement}"/> containing all measurements.</returns>
        public IEnumerable<Measurement> QueryAll()
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                var items = conn.Query<Measurement>(
                    "SELECT Id, Timestamp as Timestamp, Sensor, Value FROM Measurements"
                );
                return items.ToList();
            }
        }
    }
}