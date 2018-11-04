using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MovieProject.Models;

namespace MovieProject.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(MovieContext context)
        {
            
            if (!context.Movies.Any() && !context.Persons.Any() && !context.Genres.Any() && !context.Series.Any())
            {
                Movie m1 = new Movie { 
                    Name = "The Lord of the Rings: The Fellowship of the Ring", 
                    ReleaseDate = new System.DateTime(2001, 12, 19),
                    Runtime = 178,
                    Description = "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth from the Dark Lord Sauron. ",
                    Budget = 93000000,
                    Revenue = 871530324,
                    Status = "Released",
                    Slug = "the-lord-of-the-rings-the-fellowship-of-the-ring"
                };
                Movie m2 = new Movie { 
                    Name = "Black Panther", 
                    ReleaseDate = new System.DateTime(2018, 2, 16),
                    Runtime = 134,
                    Description = "T'Challa, heir to the hidden but advanced kingdom of Wakanda, must step forward to lead his people into a new future and must confront a challenger from his country's past. ",
                    Budget = 200000000,
                    Revenue = 1347071259,
                    Status = "Released",
                    Slug = "black-panther"
                };
                
                context.Add(m1);
                context.Add(m2);

                Person a1 = new Person { 
                    FirstName = "Elijah", 
                    Surname = "Wood",
                    BirthDate = new System.DateTime(1981, 1, 28),
                    Slug = "elijah-wood" 
                };
                Person a2 = new Person { 
                    FirstName = "Andy", 
                    Surname = "Serkis",
                    BirthDate = new System.DateTime(1964, 4, 20),
                    Slug = "andy-serkis"  
                };
                Person a3 = new Person { 
                    FirstName = "Chadwick", 
                    Surname = "Boseman", 
                    BirthDate = new System.DateTime(1977, 11, 29),
                    Slug = "chadwick-boseman"  
                };
                Person a4 = new Person { 
                    FirstName = "Peter", 
                    Surname = "Dinklage", 
                    BirthDate = new System.DateTime(1969, 6, 11),
                    Slug = "peter-dinklage"  
                };
                Person a5 = new Person { 
                    FirstName = "Emilia", 
                    Surname = "Clarke", 
                    BirthDate = new System.DateTime(1986, 10, 23),
                    Slug = "emilia-clarke" 
                };
                
                context.Add(a1);
                context.Add(a2);
                context.Add(a3);
                context.Add(a4);
                context.Add(a5);

                Genre g1 = new Genre {
                    Name = "Adventure"
                };
                Genre g2 = new Genre {
                    Name = "Drama"
                };
                Genre g3 = new Genre {
                    Name = "Fantasy"
                };
                Genre g4 = new Genre {
                    Name = "Action"
                };
                Genre g5 = new Genre {
                    Name = "Crime"
                };
                
                context.Add(g1);
                context.Add(g2);
                context.Add(g3);
                context.Add(g4);
                context.Add(g5);

                MovieCredits mc1 = new MovieCredits();
                mc1.Movie = m1;
                mc1.Person = a1;
                mc1.Character = "Frodo Baggins";

                MovieCredits mc2 = new MovieCredits();
                mc2.Movie = m1;
                mc2.Person = a2;
                mc2.Character = "Gollum (Voice)";
                
                MovieCredits mc3 = new MovieCredits();
                mc3.Movie = m2;
                mc3.Person = a2;
                mc3.Character = "Ulysses Klaue";

                MovieCredits mc4 = new MovieCredits();
                mc4.Movie = m2;
                mc4.Person = a3;
                mc4.Character = "T'Challa / Black Panther";

                context.Add(mc1);
                context.Add(mc2);
                context.Add(mc3);
                context.Add(mc4);

                MovieGenre mg1 = new MovieGenre();
                mg1.Movie = m1;
                mg1.Genre = g1;

                MovieGenre mg2 = new MovieGenre();
                mg2.Movie = m1;
                mg2.Genre = g2;

                MovieGenre mg3 = new MovieGenre();
                mg3.Movie = m1;
                mg3.Genre = g3;

                MovieGenre mg4 = new MovieGenre();
                mg4.Movie = m2;
                mg4.Genre = g1;

                MovieGenre mg5 = new MovieGenre();
                mg5.Movie = m2;
                mg5.Genre = g4;
                
                context.Add(mg1);
                context.Add(mg2);
                context.Add(mg3);
                context.Add(mg4);
                context.Add(mg5);

                Series s1 = new Series() {
                    Name = "Game of Thrones",
                    Description = "Nine noble families fight for control over the mythical lands of Westeros, while an ancient enemy returns after being dormant for thousands of years. ",
                    NumberOfSeasons = 2,
                    NumberOfEpisodes = 4,
                    FirstAirDate = new System.DateTime(2011, 4, 17),
                    LastAirDate = new System.DateTime(2017, 8, 27),
                    EpisodeRunTime = 57,
                    OriginalLanguage = "English",
                    Status = "Running",
                    Slug = "game-of-thrones"
                };

                context.Add(s1);
                context.Add(mg4);
                context.Add(mg5);

                SeriesGenre sg1 = new SeriesGenre();
                sg1.Series = s1;
                sg1.Genre = g1;

                SeriesGenre sg2 = new SeriesGenre();
                sg2.Series = s1;
                sg2.Genre = g4;

                SeriesGenre sg3 = new SeriesGenre();
                sg3.Series = s1;
                sg3.Genre = g5;

                context.Add(sg1);
                context.Add(sg2);
                context.Add(sg3);

                Season se1 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 1",
                    AirDate = new System.DateTime(2011, 4, 17),
                    SeasonNumber = 1,
                    EpisodeCount = 2
                };

                Season se2 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 2",
                    AirDate = new System.DateTime(2012, 4, 1),
                    SeasonNumber = 2,
                    EpisodeCount = 2
                };

                context.Add(se1);
                context.Add(se2);

                Episode ep1 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "Winter Is Coming",
                    AirDate = new System.DateTime(2011, 4, 17),
                    Description = "Jon Arryn, the Hand of the King, is dead. King Robert Baratheon plans to ask his oldest friend, Eddard Stark, to take Jon's place. Across the sea, Viserys Targaryen plans to wed his sister to a nomadic warlord in exchange for an army. ",
                    SeasonNumber = 1,
                    EpisodeNumber = 1,
                    Runtime = 61
                };
                Episode ep2 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "The Kingsroad",
                    AirDate = new System.DateTime(2011, 4, 17),
                    Description = "While Bran recovers from his fall, Ned takes only his daughters to King's Landing. Jon Snow goes with his uncle Benjen to the Wall. Tyrion joins them. ",
                    SeasonNumber = 1,
                    EpisodeNumber = 2,
                    Runtime = 55
                };
                Episode ep11 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The North Remembers",
                    AirDate = new System.DateTime(2011, 4, 17),
                    Description = "Tyrion arrives at King's Landing to take his father's place as Hand of the King. Stannis Baratheon plans to take the Iron Throne for his own. Robb tries to decide his next move in the war. The Night's Watch arrive at the house of Craster. ",
                    SeasonNumber = 2,
                    EpisodeNumber = 1,
                    Runtime = 52
                };
                Episode ep12 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The Night Lands",
                    AirDate = new System.DateTime(2011, 4, 17),
                    Description = "Arya makes friends with Gendry. Tyrion tries to take control of the Small Council. Theon arrives at his home, Pyke, in order to persuade his father into helping Robb with the war. Jon tries to investigate Craster's secret. ",
                    SeasonNumber = 2,
                    EpisodeNumber = 2,
                    Runtime = 53
                };
                
                context.Add(ep1);
                context.Add(ep2);
                context.Add(ep11);
                context.Add(ep12);

                EpisodeCredits epc1 = new EpisodeCredits();
                epc1.Episode = ep1;
                epc1.Person = a4;
                epc1.Character = "Tyrion Lannister";

                EpisodeCredits epc2 = new EpisodeCredits();
                epc2.Episode = ep1;
                epc2.Person = a5;
                epc2.Character = "Daenerys Targaryen";
                
                EpisodeCredits epc3 = new EpisodeCredits();
                epc3.Episode = ep2;
                epc3.Person = a4;
                epc3.Character = "Tyrion Lannister";
                
                EpisodeCredits epc4 = new EpisodeCredits();
                epc4.Episode = ep2;
                epc4.Person = a5;
                epc4.Character = "Daenerys Targaryen";
                
                EpisodeCredits epc5 = new EpisodeCredits();
                epc5.Episode = ep11;
                epc5.Person = a4;
                epc5.Character = "Tyrion Lannister";
                
                EpisodeCredits epc6 = new EpisodeCredits();
                epc6.Episode = ep11;
                epc6.Person = a5;
                epc6.Character = "Daenerys Targaryen";
                
                EpisodeCredits epc7 = new EpisodeCredits();
                epc7.Episode = ep12;
                epc7.Person = a4;
                epc7.Character = "Tyrion Lannister";
                
                EpisodeCredits epc8 = new EpisodeCredits();
                epc8.Episode = ep12;
                epc8.Person = a5;
                epc8.Character = "Daenerys Targaryen";
                
                context.Add(epc1);
                context.Add(epc2);
                context.Add(epc3);
                context.Add(epc4);
                context.Add(epc5);
                context.Add(epc6);
                context.Add(epc7);
                context.Add(epc8);

                context.SaveChanges();
            }
        }
    }
}