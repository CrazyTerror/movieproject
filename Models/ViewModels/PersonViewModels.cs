using System;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class EditPersonInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string BirthPlace { get; set; }
        public Gender Gender { get; set; }
        public string Biography { get; set; }
        public IFormFile Poster { get; set; }

        public void MapToModel(Person person)
        {
            person.FirstName = FirstName;
            person.Surname = Surname;
            person.BirthDate = BirthDate;
            person.DeathDate = DeathDate;
            person.Gender = Gender;
            person.BirthPlace = BirthPlace;
            person.Biography = Biography;
            person.UpdatedAt = DateTime.Now;
        }
    }

    public class EditPersonCreditViewModel
    {
        public string Character { get; set; }
        public string Attribute { get; set; }
    }

    public class FilmItemRelease
    {
        public int Id { get; set; }
        public int FicId { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Discriminator { get; set; }
        public string Slug { get; set; }
        public string Character { get; set; }
        public PartType PartType { get; set; }
    }
}