using System.ComponentModel.DataAnnotations;

namespace MovieProject.Models
{
    public enum Status
    {
        Development = 1,
        [Display(Name = "Pre Production")]
        PreProduction = 2,
        Production = 3,
        [Display(Name = "Post Production")]
        PostProduction = 4,
        Completed = 5,
        Released = 6,
        Running = 7,
        Ended = 8,
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3,
    }

    public enum PartType
    {
        Cast = 1,
        Composer = 2,
        [Display(Name = "Costume Designer")]
        CostumeDesigner = 3,
        Director = 4,
        Editor = 5,
        Producer = 6,
        Writer = 7,
    }
}