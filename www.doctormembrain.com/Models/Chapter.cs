using System;
using System.Collections.Generic;

namespace www.doctormembrain.com.Models
{
    public partial class Chapter
    {
        public Chapter()
        {
            Image = new HashSet<Image>();
            Reference = new HashSet<Reference>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
        public int WikiId { get; set; }

        public virtual Wiki Wiki { get; set; }
        public virtual ICollection<Image> Image { get; set; }
        public virtual ICollection<Reference> Reference { get; set; }
    }
}
