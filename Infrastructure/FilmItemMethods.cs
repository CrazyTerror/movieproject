using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class FilmItemMethods
    {
        public static string CalculateSeriesTotalRuntime(Series series)
        {
            int? totalMinutesRuntime = 0;

            foreach (var episodes in series.Seasons.Select(e => e.Episodes))
            {
                foreach (var episode in episodes)
                {
                    if (episode.Runtime != null)
                    {
                        totalMinutesRuntime += episode.Runtime;
                    }
                }
            }
            
            if (totalMinutesRuntime == null)
            {
                return string.Format("");
            }

            int? days = totalMinutesRuntime / 1440;
            int? hours = (totalMinutesRuntime % 1440)/60;
            int? minutes = totalMinutesRuntime % 60;

            if (days != 0)
            {
                return string.Format("{0} days, {1} hours, {2} minutes", days, hours, minutes);
            } else if (days == 0 && hours != 0)
            {
                return string.Format("{0} hours, {1} minutes", hours, minutes);
            } else 
            {
                return string.Format("{0} minutes", minutes);
            }
        }

        public static string CalculateSeasonTotalRuntime(Season season)
        {
            int? totalMinutesRuntime = 0;

            foreach (var episode in season.Episodes)
            {
                if (episode.Runtime != null)
                {
                    totalMinutesRuntime += episode.Runtime;
                }
            }

            if (totalMinutesRuntime == null)
            {
                return string.Format("");
            }

            int? days = totalMinutesRuntime / 1440;
            int? hours = (totalMinutesRuntime % 1440)/60;
            int? minutes = totalMinutesRuntime % 60;

            if (days != 0)
            {
                return string.Format("{0} days, {1} hours, {2} minutes", days, hours, minutes);
            } else if (days == 0 && hours != 0)
            {
                return string.Format("{0} hours, {1} minutes", hours, minutes);
            } else 
            {
                return string.Format("{0} minutes", minutes);
            }
        }

        public static void SaveFilmItemGenres(MovieContext _ctx, FilmItem filmItem, string genre)
        {
            FilmItemGenre sg = new FilmItemGenre()
            {
                FilmItem = filmItem,
                GenreId = Int32.Parse(genre)
            };

            _ctx.FilmItemGenres.Add(sg);
            _ctx.SaveChanges();
        }

        public static void SaveFilmItemCredits(MovieContext _ctx, FilmItem filmItem, Person person, int partType, string character)
        {
            character = (string.IsNullOrWhiteSpace(character) ? null : character);
            var personAlreadyCreditedInDepartment = _ctx.FilmItemCredits.Where(p => p.Person == person).Where(f => f.FilmItem == filmItem).Where(fic => fic.PartType == (PartType) partType).FirstOrDefault();

            if (personAlreadyCreditedInDepartment == null)
            {
                SaveFilmItemMember(_ctx, filmItem, person, partType, character);
            }
        }

        public static void SaveFilmItemMember(MovieContext _ctx, FilmItem filmItem, Person person, int partType, string character)
        {
            FilmItemCredits fic = new FilmItemCredits
            {
                FilmItem = filmItem,
                Person = person,
                Character = character,
                PartType = (PartType) partType
            };

            _ctx.FilmItemCredits.Add(fic);
            _ctx.SaveChanges();
        }

        public static void EditFilmItemCredit(MovieContext _ctx, FilmItemCredits filmItemCredit, int partType, string character)
        {
            character = (partType == 1 && !string.IsNullOrWhiteSpace(character) ? character : null);
            var personAlreadyCreditedInDepartment = _ctx.FilmItemCredits.Where(p => p.Person == filmItemCredit.Person)
                                                                        .Where(f => f.FilmItem == filmItemCredit.FilmItem)
                                                                        .Where(fic => fic.PartType == (PartType) partType)
                                                                        .Where(fp => fp.Id != filmItemCredit.Id)
                                                                        .FirstOrDefault();
            
            _ctx.FilmItemCredits.Attach(filmItemCredit);

            if (personAlreadyCreditedInDepartment == null)
            {
                filmItemCredit.Character = character;
                filmItemCredit.PartType = (PartType) partType;
                filmItemCredit.UpdatedAt = DateTime.Now;
            } 

            _ctx.SaveChanges();
        }

        public static Series SaveSeriesInfoAfterCreateEpisode(MovieContext _ctx, Series series)
        {
            // Change episode count in series
            _ctx.Series.Attach(series);
            if (series.Series_EpisodeCount == null)
            {
                series.Series_EpisodeCount = 1;
            } else
            {
                series.Series_EpisodeCount++;
            }
            series.UpdatedAt = DateTime.Now;

            _ctx.SaveChanges();

            return series;
        }

        public static Season SaveSeasonInfoAfterCreateEpisode(MovieContext _ctx, Season season)
        {
            // Change episode count in season
            _ctx.Seasons.Attach(season);
            if (season.Season_EpisodeCount == null)
            {
                season.Season_EpisodeCount = 1;
            } else 
            {
                season.Season_EpisodeCount++;
            }
            season.UpdatedAt = DateTime.Now;
            
            _ctx.SaveChanges();

            return season;
        }

        public static Series SaveSeriesInfoAfterCreateSeason(MovieContext _ctx, Series series)
        {
            // Change amount of seasons in series
            _ctx.Series.Attach(series);
            if (series.Series_SeasonCount == null)
            {
                series.Series_SeasonCount = 1;
            } else
            {
                series.Series_SeasonCount++;
            }
            series.UpdatedAt = DateTime.Now;

            _ctx.SaveChanges();

            return series;
        }

        public static void EditSeriesAndSeasonAfterDeleteEpisode(MovieContext _ctx, Series series, Season season)
        {
            _ctx.Attach(series);
            series.Series_EpisodeCount--;
            series.UpdatedAt = DateTime.Now;

            _ctx.Attach(season);
            season.Season_EpisodeCount--;
            season.UpdatedAt = DateTime.Now;

            _ctx.SaveChanges();
        }

        public static void EditSeriesInfoAfterDeleteSeason(MovieContext _ctx, Series series, Season season)
        {
            _ctx.Attach(series);
            series.Series_SeasonCount--;
            series.Series_EpisodeCount = series.Series_EpisodeCount - season.Season_EpisodeCount;
            series.UpdatedAt = DateTime.Now;

            _ctx.SaveChanges();
        }

        public static void AddRating(MovieContext _ctx, FilmItem filmItem, int rating, string user)
        {
            UserRating userRating = new UserRating
            {
                ApplicationUserId = user,
                FilmItem = filmItem,
                Rating = rating
            };
            _ctx.UserRatings.Add(userRating);
            _ctx.SaveChanges();
        }

        public static void AlterFilmItemAverage(MovieContext _ctx, FilmItem filmItem, int rating)
        {
            _ctx.Attach(filmItem);
            if (filmItem.VoteCount == null && filmItem.VoteAverage == null)
            {
                filmItem.VoteCount = 1;
                filmItem.VoteAverage = rating;
            } else {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount++;
                filmItem.VoteAverage = (totalRating + rating) / filmItem.VoteCount;
            }
            
            filmItem.UpdatedAt = DateTime.Now;
            _ctx.SaveChanges();
        }

        public static void CalculateFilmItemAverageAfterDeletion(MovieContext _ctx, FilmItem filmItem, int rating)
        {
            _ctx.Attach(filmItem);

            if (filmItem.VoteCount == 1)
            {
                filmItem.VoteCount = null;
                filmItem.VoteAverage = null;
            } else 
            {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount--;
                filmItem.VoteAverage = (totalRating - rating) / filmItem.VoteCount;
            }
            filmItem.UpdatedAt = DateTime.Now;

            _ctx.SaveChanges();
        }
    }
}