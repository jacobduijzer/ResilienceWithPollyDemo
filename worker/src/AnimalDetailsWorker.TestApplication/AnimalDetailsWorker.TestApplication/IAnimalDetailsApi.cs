using BuggyAnimalDetailsApi.Api;
using Refit;

namespace AnimalDetailsWorker.TestApplication;

public interface IAnimalDetailsApi
{
   [Get("/animaldetails/{animalId}")]
   Task<AnimalDetails> GetAnimalDetails(int animalId);
}