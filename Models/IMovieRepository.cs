using System.Linq;

namespace MovieProject.Models
{
    public interface IMovieRepository
    {
        IQueryable<Movie> Movies { get; }
    }
}