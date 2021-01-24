using System;
using System.Collections.Generic;

namespace www.doctormembrain.com.Models
{
    public partial class Wiki
    {
        public Wiki()
        {
            Chapter = new HashSet<Chapter>();
            Image = new HashSet<Image>();
            Reference = new HashSet<Reference>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<Chapter> Chapter { get; set; }
        public virtual ICollection<Image> Image { get; set; }
        public virtual ICollection<Reference> Reference { get; set; }
    }
}
