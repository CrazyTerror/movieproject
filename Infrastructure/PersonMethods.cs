using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Data;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class PersonMethods
    {
        public static int CalculatePersonAge(Person person)
        {
            var today = DateTime.Today;
            var birthDate = person.BirthDate;

            var age = today.Year - birthDate.Value.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public static int GetRandomBackground(Person person)
        {
            List<int> filmItemIds = new List<int>();

            foreach (var filmItem in person.FilmItemCredits)
            {
                filmItemIds.Add(filmItem.FilmItemId);
            }
            
            var filmItemId = 0;
            if (filmItemIds.Count > 0)
            {
                Random rand = new Random();
                int i = rand.Next(filmItemIds.Count);
                filmItemId = filmItemIds[i];
            }

            return filmItemId;
        }

        public static PersonWatchedByUser GetUserStats(MovieContext _ctx, Person person, string user)
        {
            List<int> filmItemIds = new List<int>();
            foreach (var filmItem in person.FilmItemCredits)
            {
                filmItemIds.Add(filmItem.FilmItemId);
            }

            var userWatchedOfPerson = 0;
            var totalFilmItemCredits = person.FilmItemCredits.GroupBy(x => x.FilmItemId).Select(x => x).Count();
            
            var userWatched = _ctx.UserWatching.Where(a => a.ApplicationUserId == user).Select(x => x.FilmItemId).ToList();
            foreach (var filmItemId in filmItemIds)
            {
                if (userWatched.Contains(filmItemId)) {
                    userWatchedOfPerson++;
                }
            }
            double percentageWatched = ((double) userWatchedOfPerson / (double) totalFilmItemCredits) * 100;
            double roundUpPercentageWatched = Math.Round(percentageWatched, MidpointRounding.AwayFromZero);

            PersonWatchedByUser personWatchedByUserModel = new PersonWatchedByUser
            {
                UserWatchedOfPerson = userWatchedOfPerson,
                TotalFilmItems = totalFilmItemCredits,
                PercentageWatchedByUser = roundUpPercentageWatched
            };

            return personWatchedByUserModel;
        }
    }
}