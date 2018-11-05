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
                    Name = "Adventure",
                    Slug = "adventure"
                };
                Genre g2 = new Genre {
                    Name = "Drama",
                    Slug = "drama"
                };
                Genre g3 = new Genre {
                    Name = "Fantasy",
                    Slug = "fantasy"
                };
                Genre g4 = new Genre {
                    Name = "Action",
                    Slug = "action"
                };
                Genre g5 = new Genre {
                    Name = "Crime",
                    Slug = "crime"
                };
                
                context.Add(g1);
                context.Add(g2);
                context.Add(g3);
                context.Add(g4);
                context.Add(g5);

                FilmItemCredits mc1 = new FilmItemCredits();
                mc1.FilmItem = m1;
                mc1.Person = a1;
                mc1.Character = "Frodo Baggins";

                FilmItemCredits mc2 = new FilmItemCredits();
                mc2.FilmItem = m1;
                mc2.Person = a2;
                mc2.Character = "Gollum (Voice)";
                
                FilmItemCredits mc3 = new FilmItemCredits();
                mc3.FilmItem = m2;
                mc3.Person = a2;
                mc3.Character = "Ulysses Klaue";

                FilmItemCredits mc4 = new FilmItemCredits();
                mc4.FilmItem = m2;
                mc4.Person = a3;
                mc4.Character = "T'Challa / Black Panther";

                context.Add(mc1);
                context.Add(mc2);
                context.Add(mc3);
                context.Add(mc4);

                FilmItemGenre mg1 = new FilmItemGenre();
                mg1.FilmItem = m1;
                mg1.Genre = g1;

                FilmItemGenre mg2 = new FilmItemGenre();
                mg2.FilmItem = m1;
                mg2.Genre = g2;

                FilmItemGenre mg3 = new FilmItemGenre();
                mg3.FilmItem = m1;
                mg3.Genre = g3;

                FilmItemGenre mg4 = new FilmItemGenre();
                mg4.FilmItem = m2;
                mg4.Genre = g1;

                FilmItemGenre mg5 = new FilmItemGenre();
                mg5.FilmItem = m2;
                mg5.Genre = g4;
                
                context.Add(mg1);
                context.Add(mg2);
                context.Add(mg3);
                context.Add(mg4);
                context.Add(mg5);

                Series s1 = new Series() {
                    Name = "Game of Thrones",
                    Description = "Nine noble families fight for control over the mythical lands of Westeros, while an ancient enemy returns after being dormant for thousands of years. ",
                    Series_SeasonCount = 2,
                    Series_EpisodeCount = 4,
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    FirstAirDate = new System.DateTime(2011, 4, 17),
                    LastAirDate = new System.DateTime(2017, 8, 27),
                    OriginalLanguage = "English",
                    Status = "Running",
                    Slug = "game-of-thrones"
                };

                context.Add(s1);
                context.Add(mg4);
                context.Add(mg5);

                FilmItemGenre sg1 = new FilmItemGenre();
                sg1.FilmItem = s1;
                sg1.Genre = g1;

                FilmItemGenre sg2 = new FilmItemGenre();
                sg2.FilmItem = s1;
                sg2.Genre = g4;

                FilmItemGenre sg3 = new FilmItemGenre();
                sg3.FilmItem = s1;
                sg3.Genre = g5;

                context.Add(sg1);
                context.Add(sg2);
                context.Add(sg3);

                Season se1 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 1",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Season_SeasonNumber = 1,
                    Season_EpisodeCount = 2
                };

                Season se2 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 2",
                    ReleaseDate = new System.DateTime(2012, 4, 1),
                    Season_SeasonNumber = 2,
                    Season_EpisodeCount = 2
                };

                context.Add(se1);
                context.Add(se2);

                Episode ep1 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "Winter Is Coming",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Jon Arryn, the Hand of the King, is dead. King Robert Baratheon plans to ask his oldest friend, Eddard Stark, to take Jon's place. Across the sea, Viserys Targaryen plans to wed his sister to a nomadic warlord in exchange for an army. ",
                    Episode_SeasonNumber = 1,
                    Episode_EpisodeNumber = 1,
                    Runtime = 61
                };
                Episode ep2 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "The Kingsroad",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "While Bran recovers from his fall, Ned takes only his daughters to King's Landing. Jon Snow goes with his uncle Benjen to the Wall. Tyrion joins them. ",
                    Episode_SeasonNumber = 1,
                    Episode_EpisodeNumber = 2,
                    Runtime = 55
                };
                Episode ep11 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The North Remembers",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Tyrion arrives at King's Landing to take his father's place as Hand of the King. Stannis Baratheon plans to take the Iron Throne for his own. Robb tries to decide his next move in the war. The Night's Watch arrive at the house of Craster. ",
                    Episode_SeasonNumber = 2,
                    Episode_EpisodeNumber = 1,
                    Runtime = 52
                };
                Episode ep12 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The Night Lands",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Arya makes friends with Gendry. Tyrion tries to take control of the Small Council. Theon arrives at his home, Pyke, in order to persuade his father into helping Robb with the war. Jon tries to investigate Craster's secret. ",
                    Episode_SeasonNumber = 2,
                    Episode_EpisodeNumber = 2,
                    Runtime = 53
                };
                
                context.Add(ep1);
                context.Add(ep2);
                context.Add(ep11);
                context.Add(ep12);

                FilmItemCredits epc1 = new FilmItemCredits();
                epc1.FilmItem = ep1;
                epc1.Person = a4;
                epc1.Character = "Tyrion Lannister";

                FilmItemCredits epc2 = new FilmItemCredits();
                epc2.FilmItem = ep1;
                epc2.Person = a5;
                epc2.Character = "Daenerys Targaryen";
                
                FilmItemCredits epc3 = new FilmItemCredits();
                epc3.FilmItem = ep2;
                epc3.Person = a4;
                epc3.Character = "Tyrion Lannister";
                
                FilmItemCredits epc4 = new FilmItemCredits();
                epc4.FilmItem = ep2;
                epc4.Person = a5;
                epc4.Character = "Daenerys Targaryen";
                
                FilmItemCredits epc5 = new FilmItemCredits();
                epc5.FilmItem = ep11;
                epc5.Person = a4;
                epc5.Character = "Tyrion Lannister";
                
                FilmItemCredits epc6 = new FilmItemCredits();
                epc6.FilmItem = ep11;
                epc6.Person = a5;
                epc6.Character = "Daenerys Targaryen";
                
                FilmItemCredits epc7 = new FilmItemCredits();
                epc7.FilmItem = ep12;
                epc7.Person = a4;
                epc7.Character = "Tyrion Lannister";
                
                FilmItemCredits epc8 = new FilmItemCredits();
                epc8.FilmItem = ep12;
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