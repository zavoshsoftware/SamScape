using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class BaseEntity : object
    {

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        [Display(Name = "فعال؟")]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public System.DateTime CreationDate { get; set; }


        [Display(Name = "اخرین تغییر")]
        public System.DateTime? LastModifiedDate { get; set; }

        [Display(Name = "IsDeleted")]
        [System.ComponentModel.DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [Display(Name = "DeletionDate")]
        public System.DateTime? DeletionDate { get; set; }

        [AllowHtml]
        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "CreationDate")]
        [NotMapped]
        public string CreationDateStr
        {
            get
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                string year = pc.GetYear(CreationDate).ToString().PadLeft(4, '0');
                string month = pc.GetMonth(CreationDate).ToString().PadLeft(2, '0');
                string day = pc.GetDayOfMonth(CreationDate).ToString().PadLeft(2, '0');
                return String.Format("{0}/{1}/{2}", year, month, day) + "  ساعت: " + CreationDate.TimeOfDay.Hours+":"+CreationDate.TimeOfDay.Minutes;
            }
        }
    }
}
