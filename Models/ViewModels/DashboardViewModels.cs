using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;

namespace MovieProject.Models
{
    public class DashboardIndexViewModel
    {
        public ApplicationUser User { get; set; }
        public int EpisodesWatched { get; set; }
        public string EpisodeTimeWatched { get; set; }
        public int SeriesWatched { get; set; }
        public int MoviesWatched { get; set; }
        public string MovieTimeWatched { get; set; }
        public List<UserWatchedFilmItemOn> RecentlyWatchedFilmItems { get; set; }
        
    }
}
