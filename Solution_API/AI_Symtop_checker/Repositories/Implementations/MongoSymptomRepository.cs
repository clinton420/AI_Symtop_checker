using MongoDB.Driver;
using AI_Symtop_checker.Model;
using AI_Symtop_checker.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AI_Symtop_checker.Repositories.Implementations
{
    public class MongoSymptomRepository : ISymptomRepository
    {
        private readonly IMongoCollection<Symptom> _symptoms;
        private readonly IMongoCollection<SymptomCheckPrediction> _predictions;

        public MongoSymptomRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("SymptomCheckerDb");
            _symptoms = database.GetCollection<Symptom>("Symptoms");
            _predictions = database.GetCollection<SymptomCheckPrediction>("Predictions");
        }

        public async Task<List<Symptom>> GetAllSymptomsAsync()
        {
            try
            {
                return await _symptoms.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error retrieving symptoms from database", ex);
            }
        }

        public async Task<Symptom> GetSymptomByIdAsync(Guid id)
        {
            try
            {
                var filter = Builders<Symptom>.Filter.Eq(s => s.Id, id);
                return await _symptoms.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving symptom with ID {id}", ex);
            }
        }

        public async Task<List<Symptom>> GetSymptomsByNameAsync(List<string> symptomNames)
        {
            try
            {
                var filter = Builders<Symptom>.Filter.In(s => s.Name, symptomNames);
                return await _symptoms.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving symptoms by names", ex);
            }
        }

        public async Task<Symptom> AddSymptomAsync(Symptom symptom)
        {
            try
            {
                await _symptoms.InsertOneAsync(symptom);
                return symptom;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new symptom", ex);
            }
        }

        public async Task<bool> UpdateSymptomAsync(Symptom symptom)
        {
            try
            {
                var filter = Builders<Symptom>.Filter.Eq(s => s.Id, symptom.Id);
                var result = await _symptoms.ReplaceOneAsync(filter, symptom);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating symptom with ID {symptom.Id}", ex);
            }
        }

        public async Task<bool> DeleteSymptomAsync(Guid id)
        {
            try
            {
                var filter = Builders<Symptom>.Filter.Eq(s => s.Id, id);
                var result = await _symptoms.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting symptom with ID {id}", ex);
            }
        }

        public async Task<SymptomCheckPrediction> CheckSymptomsAsync(List<string> symptoms)
        {
            try
            {
                // First, validate that all symptoms exist in the database
                var existingSymptoms = await GetSymptomsByNameAsync(symptoms);
                if (existingSymptoms.Count != symptoms.Count)
                {
                    var foundSymptomNames = existingSymptoms.Select(s => s.Name).ToList();
                    var notFoundSymptoms = symptoms.Except(foundSymptomNames);
                    throw new Exception($"Some symptoms were not found in the database: {string.Join(", ", notFoundSymptoms)}");
                }

                // Create a new prediction
                var prediction = new SymptomCheckPrediction
                {
                    Id = Guid.NewGuid(),
                    Symptoms = symptoms,
                    TimestampUtc = DateTime.UtcNow
                };

                // Store the prediction in MongoDB
                await _predictions.InsertOneAsync(prediction);

                return prediction;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking symptoms", ex);
            }
        }
    }
}