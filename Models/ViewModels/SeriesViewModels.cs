using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class SeriesDetailsViewModel
    {
        public Series Series { get; set; }
        public string[] Genres { get; set; }
        public string ReleaseYear { get; set; }
        public string PremiereDate { get; set; }
        public string TotalRuntime { get; set; }
        public int CommentCount { get; set; }
        public int ListCount { get; set; }
        public List<Episode> RecentlyAiredEpisodes { get; set; }
    }

    public class EditSeriesInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }

        public void MapToModel(Series series)
        {
            series.Name = Name;
            series.Description = Description;
            series.FirstAirDate = FirstAirDate;
            series.ReleaseDate = FirstAirDate;
            series.UpdatedAt = DateTime.Now;
        }
    }
}