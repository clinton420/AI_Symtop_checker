using Microsoft.AspNetCore.Mvc;
using AI_Symtop_checker.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AI_Symtop_checker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestConnectionController : ControllerBase
    {
        private readonly MongoDbConfiguration _mongoConfig;

        public TestConnectionController(IOptions<MongoDbConfiguration> mongoConfig)
        {
            _mongoConfig = mongoConfig.Value;
        }

        [HttpGet]
        public IActionResult TestMongo()
        {
            try
            {
                var client = new MongoClient(_mongoConfig.ConnectionString);
                var database = client.GetDatabase(_mongoConfig.DatabaseName);
                var collections = database.ListCollectionNames().ToList();

                return Ok(new
                {
                    message = "MongoDB Connection Successful!",
                    collections = collections
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Connection Failed: {ex.Message}");
            }
        }
    }
}