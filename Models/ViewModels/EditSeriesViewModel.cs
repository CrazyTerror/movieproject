using System;

namespace MovieProject.Models
{
    public class EditSeriesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? FirstAirDate { get; set; }
    }
}