using System;
using Microsoft.AspNetCore.Http;

namespace MovieProject.Models
{
    public class EditMediaViewModel
    {
        public int Id { get; set; }
        public string Facebook { get; set; }
        public string IMDB { get; set; }
        public string Instagram { get; set; }
        public string OfficialSite { get; set; }
        public string TMDB { get; set; }
        public string Trakt { get; set; }
        public string Twitter { get; set; }
        public string Wikipedia { get; set; }

        public void MapToModel(Media media)
        {
            media.Facebook = Facebook;
            media.IMDB = IMDB;
            media.Instagram = Instagram;
            media.OfficialSite = OfficialSite;
            media.TMDB = TMDB;
            media.Trakt = Trakt;
            media.Twitter = Twitter;
            media.Wikipedia = Wikipedia;
        }
    }
}