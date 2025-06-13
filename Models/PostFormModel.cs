// File: Models/ViewModels/PostFormModel.cs

using System.ComponentModel.DataAnnotations;

namespace MarketingIT.Models.ViewModels
{
    public class PostFormModel
    {
        [Required]
        public PostType Type { get; set; }

        // for Post + Article
        public string Text { get; set; }
        public IFormFile Photo { get; set; }

        // for Survey
        public string SurveyQuestion { get; set; }

        // store 3 answers individually
        public string SurveyOption1 { get; set; }
        public string SurveyOption2 { get; set; }
        public string SurveyOption3 { get; set; }
        public string[] SurveyOptions => new[]
         {
             SurveyOption1?.Trim() ?? "",
             SurveyOption2?.Trim() ?? "",
             SurveyOption3?.Trim() ?? ""
         };
    }

}
