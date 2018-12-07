using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieProject.Models
{
    public class FilmItemCreateParentsViewModel
    {
        public SelectList Languages { get; set; }
        public SelectList Genres { get; set; }
        public SelectList Countries { get; set; }
    }

    public class FilmItemCommentsViewModel
    {
        public FilmItem FilmItem { get; set; }
        public List<Review> Comments { get; set; }
        public string ReleaseYear { get; set; }
    }

    public class FilmItemListsViewModel
    {
        public FilmItem FilmItem { get; set; }
        public string ReleaseYear { get; set; }
        public string EpisodeString { get; set; }
    }

    public class FilmItemAddGenresViewModel
    {
        public FilmItem FilmItem { get; set; }
        public SelectList Genres { get; set; }
    }

    public class FilmItemListsModalViewModel
    {
        public List<List> Lists { get; set; }
        public string FilmItem { get; set; }
        public int FilmItemId { get; set; }
        public int ListsWithFilmItem { get; set; }
    }
}