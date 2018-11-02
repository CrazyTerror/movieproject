using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MovieProject.Models
{
    public enum Status
    {
        
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; } = null;
        public int? Runtime { get; set; } = null;
        public string Description { get; set; }
        public int? Budget { get; set; } = null;
        public int? Revenue { get; set; } = null;
        public int? VoteCount { get; set; } = null;
        public float? VoteAverage { get; set; } = null;
        public string Status { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<MovieCredits> MovieCredits { get; set; }
        public ICollection<MovieGenre> MovieGenre { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; } = null;
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<MovieCredits> MovieCredits { get; set; }
        public ICollection<EpisodeCredits> EpisodeCredits { get; set; }
    }
    
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<MovieGenre> MovieGenre { get; set; }
        public ICollection<SeriesGenre> SeriesGenre { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Series
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? NumberOfSeasons { get; set; } = null;
        public int? NumberOfEpisodes { get; set; } = null;
        public DateTime? FirstAirDate { get; set; } = null;
        public DateTime? LastAirDate { get; set; } = null;
        public int? EpisodeRunTime { get; set; } = null;
        public string OriginalLanguage { get; set; }
        public string Status { get; set; }
        public int? VoteCount { get; set; } = null;
        public float? VoteAverage { get; set; } = null;
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Season> Seasons { get; set; }
        public ICollection<SeriesGenre> SeriesGenre { get; set; }
    }

    public class Season
    {
        public int Id { get; set; }
        public int? SeasonNumber { get; set; } = null;
        public int? EpisodeCount { get; set; } = null;
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? AirDate { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int SeriesId { get; set; }
        public Series Series { get; set; }

        public ICollection<Episode> Episodes { get; set; }
    }

    public class Episode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? AirDate { get; set; } = null;
        public string Description { get; set; }
        public int? EpisodeNumber { get; set; } = null;
        public int? SeasonNumber { get; set; } = null;
        public int? Runtime { get; set; } = null;
        public int? VoteCount { get; set; } = null;
        public float? VoteAverage { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int SeasonId { get; set; }
        public Season Season { get; set; }
        
        public ICollection<EpisodeCredits> EpisodeCredits { get; set; }
    }

    public class MovieCredits
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public string Character { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class MovieGenre
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class SeriesGenre
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public Series Series { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class EpisodeCredits
    {
        public int Id { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }

        public string Character { get; set; }
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
        public int Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Trivia
    {
        public int Id { get; set; }
        public int ??? { get; set }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Photo
    {
        public int Id { get; set; }
        public int ??? { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Video
    {
        public int Id { get; set; }
        public int ??? { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    */

    //TO DO: Lists, TV Series, TV Episodes, more attributes
}