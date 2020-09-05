using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ContactUs: BaseEntity
    {
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.ContactUs))]
        [MaxLength(200, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.ContactUs))]
        [MaxLength(200, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "Required")]
        public string Email { get; set; }
        [Display(Name = "Message", ResourceType = typeof(Resources.Models.ContactUs))]
        [MaxLength(200, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(Resources.Messages))]
        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "Required")]
        [AllowHtml]
        [UIHint("RichText")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        [Display(Name = "IsRead", ResourceType = typeof(Resources.Models.ContactUs))]
        public bool IsRead { get; set; }

        [Display(Name = "Mobile", ResourceType = typeof(Resources.Models.User))]
        public string Mobile { get; set; }
    }
}