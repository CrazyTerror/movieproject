using System;

namespace MovieProject.Models
{
    public class EditMovieInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}