using Microsoft.AspNetCore.Mvc;

namespace BuggyAnimalDetailsApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalDetailsController : ControllerBase
{
    private readonly ILogger<AnimalDetailsController> _logger;

    public AnimalDetailsController(ILogger<AnimalDetailsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("/animaldetails/{animalId}")]
    public IActionResult Get(
        [FromServices] FakeAnimalGenerator fakeAnimalGenerator,
        int animalId)
    {
        var animalDetails = fakeAnimalGenerator.GetDetails(animalId);
        return Ok(animalDetails);
    }
}