using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieProject.Models
{
    public class FilmItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; } = null;
        public int? Runtime { get; set; } = null;
        [EnumDataType(typeof(Status))]
        public Status? Status { get; set; } = null;
        public string Description { get; set; }
        public string OriginalLanguage { get; set; }
        public int? VoteCount { get; set; } = null;
        public float? VoteAverage { get; set; } = null;
        public string Slug { get; set; }
        public string Discriminator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<FilmItemCredits> FilmItemCredits { get; set; }
        public ICollection<FilmItemGenre> FilmItemGenres { get; set; }
        public ICollection<Media> Media { get; set; }
        public ICollection<Trivia> Trivia { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Video> Videos { get; set; }
    }

    public class Movie : FilmItem
    {
        public int? Budget { get; set; } = null;
        public int? Revenue { get; set; } = null;
    }

    public class Series : FilmItem
    {
        public int? Series_SeasonCount { get; set; } = null;
        public int? Series_EpisodeCount { get; set; } = null;
        public DateTime? FirstAirDate { get; set; } = null;
        public DateTime? LastAirDate { get; set; } = null;

        public ICollection<Season> Seasons { get; set; }
    }

    public class Season : FilmItem
    {
        public int? Season_SeasonNumber { get; set; } = null;
        public int? Season_EpisodeCount { get; set; } = null;

        public int SeriesId { get; set; }
        public Series Series { get; set; }

        public ICollection<Episode> Episodes { get; set; }
    }

    public class Episode : FilmItem
    {
        public int? Episode_SeasonNumber { get; set; } = null;
        public int? Episode_EpisodeNumber { get; set; } = null;

        public int SeasonId { get; set; }
        public Season Season { get; set; }
    }

    public class Media
    { // To Person???
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string Trakt { get; set; }
        public string OfficialSite { get; set; }
        public string Wikipedia { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; } = null;
        public DateTime? DeathDate { get; set; } = null;
        public string BirthPlace { get; set; }
        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; } = null;
        public string Biography { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<FilmItemCredits> FilmItemCredits { get; set; }
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

    public class FilmItemCredits
    {
        public int Id { get; set; }
        public int FilmItemId { get; set; }
        public FilmItem FilmItem { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        //public PartType PartType { get; set; }
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

    /*public class UserRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class List
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    
    public class ListItems
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public List List { get; set; }
        public int ?? { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Comment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Review Review { get; set; }
        public string Comment { get; set; }
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