using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class EpisodeIndexViewModel
    {
        public List<Episode> Episodes { get; set; }
        public string SeriesName { get; set; }
        public string SeasonName { get; set; }
    }

    public class EpisodeDetailsViewModel
    {
        public Episode Episode { get; set; }
        public string[] Genres { get; set; }
        public string SeriesName { get; set; }
        public string SeasonName { get; set; }
        public int? EpisodeCount { get; set; }
        public string EpisodeNumber { get; set; }
        public int CommentCount { get; set; }
        public int ListCount { get; set; }
    }

    public class EditEpisodeInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Banner { get; set; }
        public int? Runtime { get; set; }

        public void MapToModel(Episode e)
        {
            e.Name = Name;
            e.Description = Description;
            e.ReleaseDate = ReleaseDate;
            e.Runtime = Runtime;
            e.UpdatedAt = DateTime.Now;
        }
    }
}