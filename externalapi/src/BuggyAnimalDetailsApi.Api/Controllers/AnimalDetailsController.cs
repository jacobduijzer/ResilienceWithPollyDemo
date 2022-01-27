using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BuggyAnimalDetailsApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalDetailsController : ControllerBase
{
    [HttpGet]
    [Route("/animaldetails/{animalId}")]
    public IActionResult Get(
        [FromServices]FakeAnimalGenerator fakeAnimalGenerator,
        [FromServices]IMemoryCache memoryCache,
        int animalId)
    {
        if (memoryCache.TryGetValue("LAST_REQUEST", out DateTime cacheValue))
        {
            if ((DateTime.Now - cacheValue).TotalSeconds <= 1)
                return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
        memoryCache.Set("LAST_REQUEST", DateTime.Now);

        HttpContext.Response.Headers.Add("x-custom-last-request", cacheValue.ToString(CultureInfo.InvariantCulture));
        
        var animalDetails = fakeAnimalGenerator.GetDetails(animalId);
        return Ok(animalDetails);
    }
}