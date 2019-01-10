using System.Collections.Generic;

namespace MovieProject.Models
{
    public class ReviewDetailsViewModel
    {
        public Review Review { get; set; }
        public List<Review> Replies { get; set; }
    }
}