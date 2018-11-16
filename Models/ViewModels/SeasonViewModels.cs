using System;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
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