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
    
    public partial class Poll
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Poll()
        {
            this.Poll_Answer = new HashSet<Poll_Answer>();
        }
    
        public int PollID { get; set; }
        public int TeacherID { get; set; }
        public string Question { get; set; }
        public Nullable<int> TotalParticipian { get; set; }
        public Nullable<int> Time { get; set; }
        public Nullable<bool> IsDone { get; set; }
    
        public virtual Teacher Teacher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Poll_Answer> Poll_Answer { get; set; }
    }
}
