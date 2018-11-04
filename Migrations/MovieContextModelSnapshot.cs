﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieProject.Models;

namespace MovieProject.Migrations
{
    [DbContext(typeof(MovieContext))]
    partial class MovieContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("MovieProject.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("MovieProject.Models.Episode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AirDate");

                    b.Property<string>("Banner");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<int?>("EpisodeNumber");

                    b.Property<string>("IMDB");

                    b.Property<string>("Name");

                    b.Property<int?>("Runtime");

                    b.Property<int>("SeasonId");

                    b.Property<int?>("SeasonNumber");

                    b.Property<string>("TMDB");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<float?>("VoteAverage");

                    b.Property<int?>("VoteCount");

                    b.Property<string>("Wikipedia");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("MovieProject.Models.EpisodeCredits", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Character");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("EpisodeId");

                    b.Property<int>("PersonId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("PersonId");

                    b.ToTable("EpisodeCredits");
                });

            modelBuilder.Entity("MovieProject.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("MovieProject.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<int?>("Budget");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<string>("Facebook");

                    b.Property<string>("IMDB");

                    b.Property<string>("Instagram");

                    b.Property<string>("Name");

                    b.Property<string>("OfficialSite");

                    b.Property<string>("Poster");

                    b.Property<DateTime?>("ReleaseDate");

                    b.Property<int?>("Revenue");

                    b.Property<int?>("Runtime");

                    b.Property<string>("Slug");

                    b.Property<string>("Status");

                    b.Property<string>("TMDB");

                    b.Property<string>("Twitter");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<float?>("VoteAverage");

                    b.Property<int?>("VoteCount");

                    b.Property<string>("Wikipedia");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieProject.Models.MovieCredits", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Character");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<int>("PersonId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("PersonId");

                    b.ToTable("MovieCredits");
                });

            modelBuilder.Entity("MovieProject.Models.MovieGenre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GenreId");

                    b.Property<int>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.HasIndex("MovieId");

                    b.ToTable("MovieGenre");
                });

            modelBuilder.Entity("MovieProject.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<string>("Biography");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("BirthPlace");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeathDate");

                    b.Property<string>("Facebook");

                    b.Property<string>("FirstName");

                    b.Property<string>("Homepage");

                    b.Property<string>("IMDB");

                    b.Property<string>("Instagram");

                    b.Property<string>("Poster");

                    b.Property<string>("Slug");

                    b.Property<string>("Surname");

                    b.Property<string>("TMDB");

                    b.Property<string>("Twitter");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Wikipedia");

                    b.HasKey("Id");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("MovieProject.Models.Season", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AirDate");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<int?>("EpisodeCount");

                    b.Property<string>("IMDB");

                    b.Property<string>("Name");

                    b.Property<string>("Poster");

                    b.Property<int?>("SeasonNumber");

                    b.Property<int>("SeriesId");

                    b.Property<string>("TMDB");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Wikipedia");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("MovieProject.Models.Series", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<int?>("EpisodeRunTime");

                    b.Property<string>("Facebook");

                    b.Property<DateTime?>("FirstAirDate");

                    b.Property<string>("IMDB");

                    b.Property<string>("Instagram");

                    b.Property<DateTime?>("LastAirDate");

                    b.Property<string>("Name");

                    b.Property<int?>("NumberOfEpisodes");

                    b.Property<int?>("NumberOfSeasons");

                    b.Property<string>("OfficialSite");

                    b.Property<string>("OriginalLanguage");

                    b.Property<string>("Poster");

                    b.Property<string>("Slug");

                    b.Property<string>("Status");

                    b.Property<string>("TMDB");

                    b.Property<string>("Twitter");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<float?>("VoteAverage");

                    b.Property<int?>("VoteCount");

                    b.Property<string>("Wikipedia");

                    b.HasKey("Id");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("MovieProject.Models.SeriesGenre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GenreId");

                    b.Property<int>("SeriesId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.HasIndex("SeriesId");

                    b.ToTable("SeriesGenre");
                });

            modelBuilder.Entity("MovieProject.Models.Episode", b =>
                {
                    b.HasOne("MovieProject.Models.Season", "Season")
                        .WithMany("Episodes")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieProject.Models.EpisodeCredits", b =>
                {
                    b.HasOne("MovieProject.Models.Episode", "Episode")
                        .WithMany("EpisodeCredits")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieProject.Models.Person", "Person")
                        .WithMany("EpisodeCredits")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieProject.Models.MovieCredits", b =>
                {
                    b.HasOne("MovieProject.Models.Movie", "Movie")
                        .WithMany("MovieCredits")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieProject.Models.Person", "Person")
                        .WithMany("MovieCredits")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieProject.Models.MovieGenre", b =>
                {
                    b.HasOne("MovieProject.Models.Genre", "Genre")
                        .WithMany("MovieGenre")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieProject.Models.Movie", "Movie")
                        .WithMany("MovieGenre")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieProject.Models.Season", b =>
                {
                    b.HasOne("MovieProject.Models.Series", "Series")
                        .WithMany("Seasons")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieProject.Models.SeriesGenre", b =>
                {
                    b.HasOne("MovieProject.Models.Genre", "Genre")
                        .WithMany("SeriesGenre")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieProject.Models.Series", "Series")
                        .WithMany("SeriesGenre")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
