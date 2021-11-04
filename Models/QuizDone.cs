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
    
    public partial class QuizDone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuizDone()
        {
            this.Quiz_QuizDone = new HashSet<Quiz_QuizDone>();
            this.Student_Answer = new HashSet<Student_Answer>();
            this.Student_QuizDone = new HashSet<Student_QuizDone>();
        }
    
        public int QuizDoneID { get; set; }
        public string Quiz_Name { get; set; }
        public int CourseID { get; set; }
        public string Questions { get; set; }
        public Nullable<int> NumOfQuestion { get; set; }
        public Nullable<double> TotalMark { get; set; }
        public Nullable<bool> MixQuestion { get; set; }
        public Nullable<int> MixQuestionNumber { get; set; }
        public Nullable<int> Time { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual Course Course { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quiz_QuizDone> Quiz_QuizDone { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Answer> Student_Answer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_QuizDone> Student_QuizDone { get; set; }
    }
}
