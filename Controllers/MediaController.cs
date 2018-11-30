using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class MediaController : Controller
    {
        private readonly MovieContext _context;

        public MediaController(MovieContext context)
        {
            _context = context;

        }

        [HttpPost("editMedia")]
        public IActionResult EditMedia(EditMediaViewModel mediaViewModel)
        {
            var media = _context.Media.Include(f => f.FilmItem).Include(p => p.Person).FirstOrDefault(m => m.Id == int.Parse(Request.Form["MediaId"]));

            _context.Media.Attach(media);
            mediaViewModel.MapToModel(media);
            _context.SaveChanges();

            if (media.FilmItem != null)
            {
                if (media.FilmItem.Discriminator == "Movie" || media.FilmItem.Discriminator == "Series") {
                    return RedirectToAction("Details", media.FilmItem.Discriminator, new { Slug = media.FilmItem.Slug});
                } else if (media.FilmItem.Discriminator == "Season") {
                    return RedirectToAction("Details", media.FilmItem.Discriminator, new { Slug = media.FilmItem.Slug, SeasonNumber = media.FilmItem.Season_SeasonNumber });
                } else if (media.FilmItem.Discriminator == "Episode") {
                    return RedirectToAction("Details", media.FilmItem.Discriminator, new { Slug = media.FilmItem.Slug, SeasonNumber = media.FilmItem.Episode_SeasonNumber, EpisodeNumber = media.FilmItem.Episode_EpisodeNumber });
                }
            } else if (media.Person != null) {
                return RedirectToAction("Details", "Person", new { Slug = media.Person.Slug});
            }
            
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
