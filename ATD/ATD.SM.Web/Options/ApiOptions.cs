using System.ComponentModel.DataAnnotations;

namespace ATD.SM.Web.Options;

public class ApiOptions
{
    [Required(ErrorMessage = "The API URL is required")]
    public required string StoreStateName { get; set; }
}