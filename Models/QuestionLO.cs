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
    
    public partial class QuestionLO
    {
        public int QuestionLOID { get; set; }
        public int QuestionID { get; set; }
        public int Qtype { get; set; }
        public int LearningOutcomeID { get; set; }
    
        public virtual LearningOutcome LearningOutcome { get; set; }
        public virtual QuestionType QuestionType { get; set; }
    }
}
