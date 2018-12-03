using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieProject.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieProject.Models
{
    public class FilmItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Release Date")]
        public DateTime? ReleaseDate { get; set; } = null;
        public int? Runtime { get; set; } = null;
        [Required]
        [EnumDataType(typeof(Status))]
        public Status? Status { get; set; }
        public string Description { get; set; }
        [DisplayName("Original Language")]
        public string OriginalLanguage { get; set; }
        public int? VoteCount { get; set; }
        public float? VoteAverage { get; set; }
        public string Slug { get; set; }
        public string Discriminator { get; set; }
        public int? Rel_SeriesId { get; set; }
        public string Rel_SeriesName { get; set; }
        public int? Season_SeasonNumber { get; set; }
        public int? Season_EpisodeCount { get; set; }
        public int? Episode_SeasonNumber { get; set; }
        public int? Episode_EpisodeNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<FilmItemCredits> FilmItemCredits { get; set; }
        public ICollection<FilmItemGenre> FilmItemGenres { get; set; }
        public Media Media { get; set; }
        public ICollection<Trivia> Trivia { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<UserRating> UserRatings { get; set; }
        public ICollection<UserWatchedFilmItemOn> UserWatchedOn { get; set; }
        public ICollection<ListItem> ListItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

    public class Movie : FilmItem
    {
        public int? Budget { get; set; }
        public int? Revenue { get; set; }
    }

    public class Series : FilmItem
    {
        public int? Series_SeasonCount { get; set; }
        public int? Series_EpisodeCount { get; set; }
        [DisplayName("First Air Date")]
        public DateTime? FirstAirDate { get; set; }
        [DisplayName("Last Air Date")]
        public DateTime? LastAirDate { get; set; }

        public ICollection<Season> Seasons { get; set; }
    }

    public class Season : FilmItem
    {
        public int SeriesId { get; set; }
        public Series Series { get; set; }

        public ICollection<Episode> Episodes { get; set; }
    }

    public class Episode : FilmItem
    {
        public int SeasonId { get; set; }
        public Season Season { get; set; }
    }

    public class Media
    { 
        public int Id { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string Trakt { get; set; }
        [DisplayName("Official Site")]
        public string OfficialSite { get; set; }
        public string Wikipedia { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public int? FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; } = null;
        public int? PersonId { get; set; }
        public Person Person { get; set; } = null;
    }

    public class Person
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        public string Surname { get; set; }
        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }
        [DisplayName("Death Date")]
        public DateTime? DeathDate { get; set; }
        [DisplayName("Place of Birth")]
        public string BirthPlace { get; set; }
        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }
        public string Biography { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<FilmItemCredits> FilmItemCredits { get; set; }
        public Media Media { get; set; }
    }
    
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<FilmItemGenre> FilmItemGenres { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class FilmItemCredits
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public PartType PartType { get; set; }
        public string Character { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    /*public class FilmItemCharacter
    {
        public int Id { get; set; }
        public int FilmItemCreditsId { get; set; }
        public FilmItemCredits FilmItemCredits { get; set; }
        public string Character { get; set; }
        public int BillingPosition { get; set; }
    }*/

    public class FilmItemGenre
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    
    public class UserRating
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class UserWatchedFilmItemOn
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public DateTime WatchedOn { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class List
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public bool Privacy { get; set; } = true;
        public int ItemCount { get; set; }
        public bool Deletable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<ListItem> ListItems { get; set; }
    }
    
    public class ListItem
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public List List { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Review
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int? ShoutId { get; set; }
        public Review Shout { get; set; } = null;
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    /* public class Reply
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public string Comment { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }*/

    public class Trivia
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Photo
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Video
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}