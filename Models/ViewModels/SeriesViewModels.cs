using System;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
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