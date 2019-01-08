using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;
using System;

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
        public List<UserViewingByDate> UserViewingByDate { get; set; }
    }

    public class UserViewings
    {
        public int EpisodesWatched { get; set; }
        public int? EpisodeTimeWatched { get; set; }
        public int MoviesWatched { get; set; }
        public int? MovieTimeWatched { get; set; }
    }

    public class UserViewingByDate
    {
        public int? TimeWatched { get; set; }
        public DateTime Date { get; set; }
        public int AmountOfMovies { get; set; }
        public int AmountOfEpisodes { get; set; }
    }
}
