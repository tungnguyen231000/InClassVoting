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
    
    public partial class Student_Answer
    {
        public int SAID { get; set; }
        public int QuizDoneID { get; set; }
        public int StudentID { get; set; }
        public Nullable<int> QuestionDoneID { get; set; }
        public int Qtype { get; set; }
        public string Answer { get; set; }
        public Nullable<bool> IsCorrect { get; set; }
    
        public virtual QuestionType QuestionType { get; set; }
        public virtual QuizDone QuizDone { get; set; }
        public virtual Student Student { get; set; }
    }
}
