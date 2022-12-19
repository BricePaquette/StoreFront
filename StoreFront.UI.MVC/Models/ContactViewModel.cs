using System.ComponentModel.DataAnnotations;

namespace StoreFront.UI.MVC.Models
{
    
    public class ContactViewModel
    {
        [Required(ErrorMessage = "*Name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; } //must be alphabetic characters

        [Required(ErrorMessage = "*Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } //Must contain an @ symbol and extention .com, .net, ect

        [Required(ErrorMessage = "*Subject is required")]
        public string Subject { get; set; } //Must contain text

        [Required(ErrorMessage = "*Message is required")]
        [DataType(DataType.MultilineText)]//Notes that the field should have a bigger text-box(text-area)
        public string Message { get; set; } //Must contain text
    }
}
