//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InClassVoting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Passage_Done
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Passage_Done()
        {
            this.QuestionDones = new HashSet<QuestionDone>();
        }
    
        public int P_DoneID { get; set; }
        public Nullable<int> ChapterID { get; set; }
        public string Text { get; set; }
        public byte[] PassageImage { get; set; }
    
        public virtual Chapter Chapter { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionDone> QuestionDones { get; set; }
    }
}
