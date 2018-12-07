using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class SeasonIndexViewModel
    {
        public Series Series { get; set; }
        public List<Season> Seasons { get; set; }
    }

    public class SeasonDetailsViewModel
    {
        public Season Season { get; set; }
        public string[] Genres { get; set; }
        public int? SeasonCount { get; set; }
        public string TotalRuntime { get; set; }
        public int CommentCount { get; set; }
        public int ListCount { get; set; }
        public string FirstEpisodeDate { get; set; }
    }

    public class EditSeasonInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? AirDate { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }

        public void MapToModel(Season season)
        {
            season.Name = Name;
            season.Description = Description;
            season.ReleaseDate = AirDate;
            season.UpdatedAt = DateTime.Now;
        }
    }

    public class PeopleOnSeries
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string CharacterName { get; set; }
        public int EpisodeCount { get; set; }
    }
}