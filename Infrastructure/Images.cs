using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class Images
    {
        public static void ReadImages(MovieContext _context, IHostingEnvironment _env, IFormFileCollection images, string type)
        {
            var poster = images["Poster"];
            var banner = images["Banner"];
            if (poster != null && poster.Length > 0)
            {
                Images.UploadAssetImage(_context, _env, poster, Path.GetFileName(poster.FileName), true, type);
            } 
            if (banner != null && banner.Length > 0)
            {
                Images.UploadAssetImage(_context, _env, banner, Path.GetFileName(poster.FileName), false, type);
            }
        }
        public static void UploadAssetImage(MovieContext _context, IHostingEnvironment _env, IFormFile image, string fileName, bool poster, string type)
        {
            int? lastAssetId = null;
            
            switch (type)
            {
                case "filmitem":
                    lastAssetId = _context.FilmItem.Last().Id;
                    break;
                case "person":
                    lastAssetId = _context.Persons.Last().Id;
                    break;
                default:
                    System.Console.WriteLine("Well Shit");
                    break;
            }

            var extension = System.IO.Path.GetExtension(fileName);
            var newFileName = lastAssetId + "" + extension;
            var path = "";

            if (poster == true)
            {
                path = Path.Combine(_env.WebRootPath, "images\\" + type + "\\poster\\") + newFileName;
            } else if (poster == false)
            {
                path = Path.Combine(_env.WebRootPath, "images\\" + type + "\\") + newFileName;
            }
            
            using (FileStream fs = System.IO.File.Create(path))
            {
                image.CopyTo(fs);
                fs.Flush();
            }
        }

        public static void EditAssetImage(MovieContext _context, IHostingEnvironment _env, int id)
        {
            // TO DO
        }

        public static void DeleteAssetImage(MovieContext _context, IHostingEnvironment _env, int id)
        {
            // TO DO
        }
    }
}