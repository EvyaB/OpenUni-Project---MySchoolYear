//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MySchoolYear.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Score
    {
        public int studentID { get; set; }
        public int courseID { get; set; }
        public byte score { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
