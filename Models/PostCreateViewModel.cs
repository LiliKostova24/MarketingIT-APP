using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;  // for IFormFile

public class PostCreateViewModel
{
    [Required]
    public string Content { get; set; }

    public List<IFormFile> Photos { get; set; }
}
