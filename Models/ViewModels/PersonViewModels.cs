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
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }
    }

    public class FilmItemRelease
    {
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Character { get; set; }
    }
}