using System;
using System.Collections.Generic;

namespace www.doctormembrain.com.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? WikiId { get; set; }
        public int? ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; }
        public virtual Wiki Wiki { get; set; }
    }
}
