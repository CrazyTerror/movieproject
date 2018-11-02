using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MovieProject.Models
{
    public class EFMovieRepository : IMovieRepository
    {
        private MovieContext context;

        public EFMovieRepository(MovieContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Movie> Movies => context.Movies.Include(movie => movie.MovieCredits).ThenInclude(moviecast => moviecast.Person);
    }
}