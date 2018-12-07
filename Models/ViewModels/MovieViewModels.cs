using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public string[] Genres { get; set; }
        public string ReleaseYear { get; set; }
        public string ReleaseDate { get; set; }
        public List<FilmItemCredits> Directors { get; set; }
        public List<FilmItemCredits> Producers { get; set; }
        public List<FilmItemCredits> Writers { get; set; }
        public List<List> Lists { get; set; }
        public int ListCount { get; set; }
        public int CommentCount { get; set; }

    }

    public class EditMovieInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }
        public DateTime ReleaseDate { get; set; }

        public void MapToModel(Movie movie)
        {
            movie.Name = Name;
            movie.Description = Description;
            movie.ReleaseDate = ReleaseDate;
            movie.UpdatedAt = DateTime.Now;
        }
    }

    public class EditMovieCreditViewModel
    {
        public string Character { get; set; }
        public PartType PartType { get; set; }
        public string Attribute { get; set; }
    }
}