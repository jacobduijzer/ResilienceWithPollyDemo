using Bogus;

namespace BuggyAnimalDetailsApi.Api;

public class FakeAnimalGenerator
{
    private readonly Random _random;

    public FakeAnimalGenerator()
    {
        _random = new Random();
    }

    public AnimalDetails GetDetails(int animalId) =>
        new Faker<AnimalDetails>()
            .RuleFor(animal => animal.AnimalId, f => animalId)
            .RuleFor(animal => animal.Name, f => $"{f.Name.FirstName()} {f.Random.Number(1, 15)}")
            .RuleFor(animal => animal.DateOfBirth, f => f.Date.Past(5))
            .Generate(1)
            .First();

}