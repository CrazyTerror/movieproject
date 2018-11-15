using System;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class EditMovieInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class EditMovieCreditViewModel
    {
        public string Character { get; set; }
        public string Attribute { get; set; }
    }
}