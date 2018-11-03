using System;

namespace MovieProject.Models
{
    public class EditSeasonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? AirDate { get; set; }
    }
}