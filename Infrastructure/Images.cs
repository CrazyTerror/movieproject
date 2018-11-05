using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class Images
    {
        public static void ReadImages(MovieContext _context, IHostingEnvironment _env, IFormFileCollection images, string type, int id = 0)
        {
            var poster = images["Poster"];
            var banner = images["Banner"];
            
            if (poster != null)
            {
                Images.UploadAssetImage(_context, _env, poster, Path.GetFileName(poster.FileName), true, type, id);
            } 
            if (banner != null)
            {
                Images.UploadAssetImage(_context, _env, banner, Path.GetFileName(banner.FileName), false, type, id);
            }
        }

        public static void UploadAssetImage(MovieContext _context, IHostingEnvironment _env, IFormFile image, string fileName, bool poster, string type, int id)
        {
            int assetId = 0;

            if (id != 0)
            {
                assetId = id;
            } else 
            {
                switch (type)
                {
                    case "filmitem":
                        assetId = _context.FilmItem.Last().Id;
                        break;
                    case "person":
                        assetId = _context.Persons.Last().Id;
                        break;
                    default:
                        System.Console.WriteLine("Well Shit");
                        break;
                }
            }

            var extension = System.IO.Path.GetExtension(fileName);
            var newFileName = assetId + "" + extension;
            var path = "";

            if (poster == true)
            {
                path = Path.Combine(_env.WebRootPath, "images\\" + type + "\\poster\\") + newFileName;
            } else if (poster == false)
            {
                path = Path.Combine(_env.WebRootPath, "images\\" + type + "\\") + newFileName;
            }
            System.Console.WriteLine(path);
            
            using (FileStream fs = System.IO.File.Create(path))
            {
                image.CopyTo(fs);
                fs.Flush();
            }
        }

        public static void DeleteAssetImage(MovieContext _context, IHostingEnvironment _env, int id)
        {
            // TO DO
        }
    }
}