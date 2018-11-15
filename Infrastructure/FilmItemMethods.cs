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
        public static string CalculateTotalRuntime(Series series)
        {
            int? totalMinutesRuntime = 0;

            foreach (var episodes in series.Seasons.Select(e => e.Episodes))
            {
                foreach (var episode in episodes)
                {
                    totalMinutesRuntime += episode.Runtime;
                }
            }
            int? days = totalMinutesRuntime / 1440;
            int? hours = (totalMinutesRuntime % 1440)/60;
            int? minutes = totalMinutesRuntime % 60;

            if (days != 0)
            {
                return string.Format("{0} days, {1} hours, {2} minutes", days, hours, minutes);
            } else 
            {
                return string.Format("{0} hours, {1} minutes", hours, minutes);
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

        public static void SaveFilmItemCredits(MovieContext _ctx, FilmItem filmItem, Person person, string character)
        {
            FilmItemCredits fic = new FilmItemCredits
            {
                FilmItem = filmItem,
                Person = person,
                Character = character
            };

            _ctx.FilmItemCredits.Add(fic);
            _ctx.SaveChanges();
        }
    }
}