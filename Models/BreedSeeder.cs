using PupV1.Data;

namespace PupV1.Models
{
    public class BreedSeeder
    {
        public static void SeeBreeds(ApplicationDbContext context)
        {
            if (!context.Breedtypes.Any())
            {
                var breeds = new List<Breedtype>
                    {
                        new Breedtype { BreedName = "Golden Retriever", Size = "L", ActivityLevel = "H", Grooming = "M", SaleCount = 0 },
                new Breedtype { BreedName = "Labrador Retriever", Size = "L", ActivityLevel = "H", Grooming = "L", SaleCount = 0 },
                new Breedtype { BreedName = "German Shepherd", Size = "L", ActivityLevel = "H", Grooming = "M", SaleCount = 0 },
                new Breedtype { BreedName = "French Bulldog", Size = "M", ActivityLevel = "M", Grooming = "L", SaleCount = 0 },
                new Breedtype { BreedName = "Beagle", Size = "M", ActivityLevel = "H", Grooming = "L", SaleCount = 0 },
                new Breedtype { BreedName = "Poodle", Size = "M", ActivityLevel = "M", Grooming = "H", SaleCount = 0 },
                new Breedtype { BreedName = "Yorkshire Terrier", Size = "S", ActivityLevel = "M", Grooming = "H", SaleCount = 0 },
                new Breedtype { BreedName = "Chihuahua", Size = "S", ActivityLevel = "L", Grooming = "L", SaleCount = 0 },
                new Breedtype { BreedName = "Pomeranian", Size = "S", ActivityLevel = "M", Grooming = "H", SaleCount = 0 },
                new Breedtype { BreedName = "Rottweiler", Size = "L", ActivityLevel = "H", Grooming = "L", SaleCount = 0 }
                    };
                context.Breedtypes.AddRange(breeds);
                context.SaveChanges();
            }
        }
    }
}
