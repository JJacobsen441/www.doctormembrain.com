using System;
using System.Collections.Generic;

namespace www.doctormembrain.com.Models
{
    public partial class Reference
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
        public int WikiId { get; set; }
        public int ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; }
        public virtual Wiki Wiki { get; set; }
    }
}
