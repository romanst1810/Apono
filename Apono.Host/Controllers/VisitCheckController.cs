using Microsoft.AspNetCore.Mvc;
using Apono.Data;

namespace Apono.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitCheckController(IDataService dataService, IConfiguration cfg) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string fileName = cfg["FileName"] ?? string.Empty;
            var path = Path.Combine(AppContext.BaseDirectory, fileName);
            var dataset = await dataService.GetDataSetAsync(path, CancellationToken.None);
            
            var results = new List<bool> { 
                await dataService.CanVisitAsync("Aco", "Armory", dataset, CancellationToken.None), // False 
                await dataService.CanVisitAsync("Aro", "City Wall", dataset, CancellationToken.None),   // True
                await dataService.CanVisitAsync("Baco", "Storage", dataset, CancellationToken.None) // True
            };
           
            return Ok(results);
        }
    }
}
