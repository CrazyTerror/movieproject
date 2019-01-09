using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;
using System;

namespace MovieProject.Models
{

    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }

    public class UserInfoViewModel
    {
        public ApplicationUser User { get; set; }
        public List<RatingDistribution> RatingDistribution { get; set; }
        public List<UserWatchedFilmItemOn> UserLatestSeenMovies { get; set; }
        public List<UserWatchedFilmItemOn> UserLatestSeenEpisodes { get; set; }
        public List<UserRating> UserFavoriteSeries { get; set; }
        public List<UserRating> UserFavoriteMovies { get; set; }
        public UserWatchedFilmItemOn LastWatchedFilmItem { get; set; }
        public FilmItem MostWatchedMovie { get; set; }
    }

    public class RatingDistribution
    {
        public int Value { get; set; }
        public int Count { get; set; }
    }

    public class MostWatchedMovie
    {
        public int Movie { get; set; }
        public int Count { get; set; }
    }

    public class UserHistoryViewModel
    {
        public List<FilmItemHistoryByDate> UserWatchedFilmItems { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class UserRatingsViewModel
    {
        public List<UserRating> Ratings { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class UserCommentsViewModel
    {
        public List<Review> Comments { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class FilmItemHistoryByDate 
    {
        public DateTime Date { get; set; }
        public List<FilmItemWatching> FilmItems { get; set; }
        public int? Runtime { get; set; }
    }

    public class FilmItemWatching
    {
        public DateTime Date { get; set; }
        public FilmItem FilmItem { get; set; }
    }
}
