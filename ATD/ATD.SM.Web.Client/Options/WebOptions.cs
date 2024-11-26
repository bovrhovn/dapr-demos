using System.ComponentModel.DataAnnotations;

namespace ATD.SM.Web.Client.Options
{
    public class WebOptions
    {
        [Required(ErrorMessage = "WebApiLink is required")]
        public required string WebApiLink { get; set; }
    }
}