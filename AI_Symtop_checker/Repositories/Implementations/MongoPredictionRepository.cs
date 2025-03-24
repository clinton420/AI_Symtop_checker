using MongoDB.Driver;
using AI_Symtop_checker.Model;
using AI_Symtop_checker.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Symtop_checker.Repositories.Implementations
{
    public class MongoPredictionRepository : IPredictionRepository
    {
        private readonly IMongoCollection<SymptomCheckPrediction> _predictions;
        private readonly ILogger<MongoPredictionRepository> _logger;

        public MongoPredictionRepository(IConfiguration configuration, ILogger<MongoPredictionRepository> logger)
        {
            _logger = logger;

            try
            {
                var connectionString = configuration.GetConnectionString("MongoDB");
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("SymptomCheckerDb");
                _predictions = database.GetCollection<SymptomCheckPrediction>("SymptomCheckPredictions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MongoPredictionRepository");
                throw;
            }
        }

        public async Task<List<SymptomCheckPrediction>> GetPredictionsAsync(DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            try
            {
                var filter = BuildDateRangeFilter(startDate, endDate);
                return await _predictions.Find(filter)
                    .SortByDescending(p => p.TimestampUtc)
                    .Skip((page - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving predictions");
                throw;
            }
        }

        public async Task<int> GetPredictionsCountAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var filter = BuildDateRangeFilter(startDate, endDate);
                return (int)await _predictions.CountDocumentsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting predictions");
                throw;
            }
        }

        public async Task<int> GetPredictionsCountByUrgencyLevelAsync(string urgencyLevel)
        {
            try
            {
                var filter = Builders<SymptomCheckPrediction>.Filter.Eq(p => p.UrgencyLevel, urgencyLevel);
                return (int)await _predictions.CountDocumentsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error counting predictions by urgency level: {urgencyLevel}");
                throw;
            }
        }

        public async Task<SymptomCheckPrediction> GetPredictionByIdAsync(Guid id)
        {
            try
            {
                var filter = Builders<SymptomCheckPrediction>.Filter.Eq(p => p.Id, id);
                return await _predictions.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving prediction with ID: {id}");
                throw;
            }
        }

        public async Task<SymptomCheckPrediction> AddPredictionAsync(SymptomCheckPrediction prediction)
        {
            try
            {
                if (prediction.Id == Guid.Empty)
                {
                    prediction.Id = Guid.NewGuid();
                }

                if (prediction.TimestampUtc == default)
                {
                    prediction.TimestampUtc = DateTime.UtcNow;
                }

                await _predictions.InsertOneAsync(prediction);
                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding prediction");
                throw;
            }
        }

        public async Task<bool> DeletePredictionAsync(Guid id)
        {
            try
            {
                var filter = Builders<SymptomCheckPrediction>.Filter.Eq(p => p.Id, id);
                var result = await _predictions.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting prediction with ID: {id}");
                throw;
            }
        }

        private FilterDefinition<SymptomCheckPrediction> BuildDateRangeFilter(DateTime? startDate, DateTime? endDate)
        {
            var builder = Builders<SymptomCheckPrediction>.Filter;
            var filter = builder.Empty;

            if (startDate.HasValue)
            {
                filter = filter & builder.Gte(p => p.TimestampUtc, startDate.Value);
            }

            if (endDate.HasValue)
            {
                filter = filter & builder.Lt(p => p.TimestampUtc, endDate.Value);
            }

            return filter;
        }
    }
}