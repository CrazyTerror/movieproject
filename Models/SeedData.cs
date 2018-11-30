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
            if (!context.Movies.Any() && !context.Persons.Any() && !context.Series.Any() && !context.Genres.Any())
            {
                Movie m1 = new Movie { 
                    Name = "The Lord of the Rings: The Fellowship of the Ring", 
                    ReleaseDate = new System.DateTime(2001, 12, 19),
                    Runtime = 178,
                    Description = "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth from the Dark Lord Sauron. ",
                    Budget = 93000000,
                    Revenue = 871530324,
                    Status = Status.Released,
                    Slug = "the-lord-of-the-rings-the-fellowship-of-the-ring"
                };
                Movie m2 = new Movie { 
                    Name = "Black Panther", 
                    ReleaseDate = new System.DateTime(2018, 2, 16),
                    Runtime = 134,
                    Description = "T'Challa, heir to the hidden but advanced kingdom of Wakanda, must step forward to lead his people into a new future and must confront a challenger from his country's past. ",
                    Budget = 200000000,
                    Revenue = 1347071259,
                    Status = Status.Released,
                    Slug = "black-panther"
                };
                
                context.FilmItem.Add(m1);
                context.FilmItem.Add(m2);
                context.SaveChanges();

                Person a1 = new Person { 
                    FirstName = "Elijah", 
                    Surname = "Wood",
                    BirthDate = new System.DateTime(1981, 1, 28),
                    Slug = "elijah-wood",
                    Gender = Gender.Male 
                };
                Person a2 = new Person { 
                    FirstName = "Andy", 
                    Surname = "Serkis",
                    BirthDate = new System.DateTime(1964, 4, 20),
                    Slug = "andy-serkis",
                    Gender = Gender.Male   
                };
                Person a3 = new Person { 
                    FirstName = "Chadwick", 
                    Surname = "Boseman", 
                    BirthDate = new System.DateTime(1977, 11, 29),
                    Slug = "chadwick-boseman",
                    Gender = Gender.Male   
                };
                Person a4 = new Person { 
                    FirstName = "Peter", 
                    Surname = "Dinklage", 
                    BirthDate = new System.DateTime(1969, 6, 11),
                    Slug = "peter-dinklage",
                    Gender = Gender.Male   
                };
                Person a5 = new Person { 
                    FirstName = "Emilia", 
                    Surname = "Clarke", 
                    BirthDate = new System.DateTime(1986, 10, 23),
                    Slug = "emilia-clarke",
                    Gender = Gender.Female  
                };
                
                context.Persons.Add(a1);
                context.Persons.Add(a2);
                context.Persons.Add(a3);
                context.Persons.Add(a4);
                context.Persons.Add(a5);
                context.SaveChanges();

                FilmItemCredits mc1 = new FilmItemCredits();
                mc1.FilmItem = m1;
                mc1.Person = a1;
                mc1.Character = "Frodo Baggins";
                mc1.PartType = PartType.Cast;

                FilmItemCredits mc2 = new FilmItemCredits();
                mc2.FilmItem = m1;
                mc2.Person = a2;
                mc2.Character = "Gollum (Voice)";
                mc2.PartType = PartType.Cast;
                
                FilmItemCredits mc3 = new FilmItemCredits();
                mc3.FilmItem = m2;
                mc3.Person = a2;
                mc3.Character = "Ulysses Klaue";
                mc3.PartType = PartType.Cast;

                FilmItemCredits mc4 = new FilmItemCredits();
                mc4.FilmItem = m2;
                mc4.Person = a3;
                mc4.Character = "T'Challa / Black Panther";
                mc4.PartType = PartType.Cast;

                context.FilmItemCredits.Add(mc1);
                context.FilmItemCredits.Add(mc2);
                context.FilmItemCredits.Add(mc3);
                context.FilmItemCredits.Add(mc4);
                context.SaveChanges();
                
                Genre g1 = new Genre {
                    Name = "Action",
                    Slug = "action"
                };
                Genre g2 = new Genre {
                    Name = "Adult",
                    Slug = "adult"
                };
                Genre g3 = new Genre {
                    Name = "Adventure",
                    Slug = "adventure"
                };
                Genre g4 = new Genre {
                    Name = "Animation",
                    Slug = "animation"
                };
                Genre g5 = new Genre {
                    Name = "Biography",
                    Slug = "biography"
                };
                Genre g6 = new Genre {
                    Name = "Comedy",
                    Slug = "comedy"
                };
                Genre g7 = new Genre {
                    Name = "Crime",
                    Slug = "crime"
                };
                Genre g8 = new Genre {
                    Name = "Documentary",
                    Slug = "documentary"
                };
                Genre g9 = new Genre {
                    Name = "Drama",
                    Slug = "drama"
                };
                Genre g10 = new Genre {
                    Name = "Family",
                    Slug = "family"
                };
                Genre g11 = new Genre {
                    Name = "Fantasy",
                    Slug = "fantasy"
                };
                Genre g12 = new Genre {
                    Name = "Film-Noir",
                    Slug = "film-noir"
                };
                Genre g13 = new Genre {
                    Name = "Game-Show",
                    Slug = "game-show"
                };
                Genre g14 = new Genre {
                    Name = "History",
                    Slug = "history"
                };
                Genre g15 = new Genre {
                    Name = "Horror",
                    Slug = "horror"
                };
                Genre g16 = new Genre {
                    Name = "Music",
                    Slug = "music"
                };
                Genre g17 = new Genre {
                    Name = "Musical",
                    Slug = "musical"
                };
                Genre g18 = new Genre {
                    Name = "Mystery",
                    Slug = "mystery"
                };
                Genre g19 = new Genre {
                    Name = "News",
                    Slug = "news"
                };
                Genre g20 = new Genre {
                    Name = "Reality-TV",
                    Slug = "reality-tv"
                };
                Genre g21 = new Genre {
                    Name = "Romance",
                    Slug = "romance"
                };
                Genre g22 = new Genre {
                    Name = "Science-Fiction",
                    Slug = "sci-fi"
                };
                Genre g23 = new Genre {
                    Name = "Short",
                    Slug = "short"
                };
                Genre g24 = new Genre {
                    Name = "Sport",
                    Slug = "sport"
                };
                Genre g25 = new Genre {
                    Name = "Talk-Show",
                    Slug = "talk-show"
                };
                Genre g26 = new Genre {
                    Name = "Thriller",
                    Slug = "thriller"
                };
                Genre g27 = new Genre {
                    Name = "War",
                    Slug = "war"
                };
                Genre g28 = new Genre {
                    Name = "Western",
                    Slug = "western"
                };
                
                context.Genres.Add(g1);
                context.Genres.Add(g2);
                context.Genres.Add(g3);
                context.Genres.Add(g4);
                context.Genres.Add(g5);
                context.Genres.Add(g6);
                context.Genres.Add(g7);
                context.Genres.Add(g8);
                context.Genres.Add(g9);
                context.Genres.Add(g10);
                context.Genres.Add(g11);
                context.Genres.Add(g12);
                context.Genres.Add(g13);
                context.Genres.Add(g14);
                context.Genres.Add(g15);
                context.Genres.Add(g16);
                context.Genres.Add(g17);
                context.Genres.Add(g18);
                context.Genres.Add(g19);
                context.Genres.Add(g20);
                context.Genres.Add(g21);
                context.Genres.Add(g22);
                context.Genres.Add(g23);
                context.Genres.Add(g24);
                context.Genres.Add(g25);
                context.Genres.Add(g26);
                context.Genres.Add(g27);
                context.Genres.Add(g28);
                context.SaveChanges();

                FilmItemGenre mg1 = new FilmItemGenre();
                mg1.FilmItem = m1;
                mg1.Genre = g3;

                FilmItemGenre mg2 = new FilmItemGenre();
                mg2.FilmItem = m1;
                mg2.Genre = g9;

                FilmItemGenre mg3 = new FilmItemGenre();
                mg3.FilmItem = m1;
                mg3.Genre = g11;

                FilmItemGenre mg4 = new FilmItemGenre();
                mg4.FilmItem = m2;
                mg4.Genre = g1;

                FilmItemGenre mg5 = new FilmItemGenre();
                mg5.FilmItem = m2;
                mg5.Genre = g3;

                FilmItemGenre mg6 = new FilmItemGenre();
                mg6.FilmItem = m2;
                mg6.Genre = g22;
                
                context.FilmItemGenres.Add(mg1);
                context.FilmItemGenres.Add(mg2);
                context.FilmItemGenres.Add(mg3);
                context.FilmItemGenres.Add(mg4);
                context.FilmItemGenres.Add(mg5);
                context.FilmItemGenres.Add(mg6);
                context.SaveChanges();

                Series s1 = new Series() {
                    Name = "Game of Thrones",
                    Description = "Nine noble families fight for control over the mythical lands of Westeros, while an ancient enemy returns after being dormant for thousands of years. ",
                    Series_SeasonCount = 2,
                    Series_EpisodeCount = 4,
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    FirstAirDate = new System.DateTime(2011, 4, 17),
                    LastAirDate = new System.DateTime(2017, 8, 27),
                    OriginalLanguage = "English",
                    Status = Status.Running,
                    Slug = "game-of-thrones"
                };

                context.FilmItem.Add(s1);
                context.SaveChanges();

                FilmItemGenre sg1 = new FilmItemGenre();
                sg1.FilmItem = s1;
                sg1.Genre = g1;

                FilmItemGenre sg2 = new FilmItemGenre();
                sg2.FilmItem = s1;
                sg2.Genre = g3;

                FilmItemGenre sg3 = new FilmItemGenre();
                sg3.FilmItem = s1;
                sg3.Genre = g9;

                context.FilmItemGenres.Add(sg1);
                context.FilmItemGenres.Add(sg2);
                context.FilmItemGenres.Add(sg3);
                context.SaveChanges();

                Season se1 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 1",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Season_SeasonNumber = 1,
                    Season_EpisodeCount = 2,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };

                Season se2 = new Season()
                {
                    SeriesId = s1.Id,
                    Name = "Season 2",
                    ReleaseDate = new System.DateTime(2012, 4, 1),
                    Season_SeasonNumber = 2,
                    Season_EpisodeCount = 2,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };

                context.FilmItem.Add(se1);
                context.FilmItem.Add(se2);
                context.SaveChanges();

                Episode ep1 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "Winter Is Coming",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Jon Arryn, the Hand of the King, is dead. King Robert Baratheon plans to ask his oldest friend, Eddard Stark, to take Jon's place. Across the sea, Viserys Targaryen plans to wed his sister to a nomadic warlord in exchange for an army. ",
                    Episode_SeasonNumber = 1,
                    Episode_EpisodeNumber = 1,
                    Runtime = 61,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };
                Episode ep2 = new Episode()
                {
                    SeasonId = se1.Id,
                    Name = "The Kingsroad",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "While Bran recovers from his fall, Ned takes only his daughters to King's Landing. Jon Snow goes with his uncle Benjen to the Wall. Tyrion joins them. ",
                    Episode_SeasonNumber = 1,
                    Episode_EpisodeNumber = 2,
                    Runtime = 55,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };
                Episode ep11 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The North Remembers",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Tyrion arrives at King's Landing to take his father's place as Hand of the King. Stannis Baratheon plans to take the Iron Throne for his own. Robb tries to decide his next move in the war. The Night's Watch arrive at the house of Craster. ",
                    Episode_SeasonNumber = 2,
                    Episode_EpisodeNumber = 1,
                    Runtime = 52,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };
                Episode ep12 = new Episode()
                {
                    SeasonId = se2.Id,
                    Name = "The Night Lands",
                    ReleaseDate = new System.DateTime(2011, 4, 17),
                    Description = "Arya makes friends with Gendry. Tyrion tries to take control of the Small Council. Theon arrives at his home, Pyke, in order to persuade his father into helping Robb with the war. Jon tries to investigate Craster's secret. ",
                    Episode_SeasonNumber = 2,
                    Episode_EpisodeNumber = 2,
                    Runtime = 53,
                    Status = Status.Released,
                    Slug = "game-of-thrones",
                    Rel_SeriesId = s1.Id,
                    Rel_SeriesName = s1.Name
                };
                
                context.FilmItem.Add(ep1);
                context.FilmItem.Add(ep2);
                context.FilmItem.Add(ep11);
                context.FilmItem.Add(ep12);
                context.SaveChanges();

                FilmItemCredits fica1 = new FilmItemCredits();
                fica1.FilmItem = s1;
                fica1.Person = a4;
                fica1.Character = "Tyrion Lannister";
                fica1.PartType = PartType.Cast;
                
                FilmItemCredits fica2 = new FilmItemCredits();
                fica2.FilmItem = s1;
                fica2.Person = a5;
                fica2.Character = "Daenerys Targaryen";
                fica2.PartType = PartType.Cast;
                
                context.FilmItemCredits.Add(fica1);
                context.FilmItemCredits.Add(fica2);

                Media media1 = new Media
                {
                    FilmItem = m1
                };
                Media media2 = new Media
                {
                    FilmItem = m2
                };
                Media media3 = new Media
                {
                    FilmItem = s1
                };
                Media media4 = new Media
                {
                    FilmItem = se1
                };
                Media media5 = new Media
                {
                    FilmItem = se2
                };
                Media media6 = new Media
                {
                    FilmItem = ep1
                };
                Media media7 = new Media
                {
                    FilmItem = ep2
                };
                Media media8 = new Media
                {
                    FilmItem = ep11
                };
                Media media9 = new Media
                {
                    FilmItem = ep12
                };
                Media media10 = new Media
                {
                    Person = a1
                };
                Media media11 = new Media
                {
                    Person = a2
                };
                Media media12 = new Media
                {
                    Person = a3
                };
                Media media13 = new Media
                {
                    Person = a4
                };
                Media media14 = new Media
                {
                    Person = a5
                };
                
                context.Media.Add(media1);
                context.Media.Add(media2);
                context.Media.Add(media3);
                context.Media.Add(media4);
                context.Media.Add(media5);
                context.Media.Add(media6);
                context.Media.Add(media7);
                context.Media.Add(media8);
                context.Media.Add(media9);
                context.Media.Add(media10);
                context.Media.Add(media11);
                context.Media.Add(media12);
                context.Media.Add(media13);
                context.Media.Add(media14);
                
                context.SaveChanges();
            }
        }
    }
}