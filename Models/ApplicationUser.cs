using Microsoft.AspNetCore.Identity;

namespace PupV1.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty ;
        public string Suburb { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CellNUm { get; set; } = string.Empty;
        public string KennelName {  get; set; } = string.Empty;

        public int? TrainerId { get; set; } 
        public int? ClientId { get; set; }
        public int? BreederId { get; set; }

        public Trainer? Trainer { get; set; }
        public Client? Client { get; set; }
        public Breeder? Breeder { get; set; }
    }
}
