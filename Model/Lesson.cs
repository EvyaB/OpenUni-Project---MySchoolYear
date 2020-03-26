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
    
    public partial class Lesson
    {
        public int lessonID { get; set; }
        public int teacherID { get; set; }
        public int courseID { get; set; }
        public int classID { get; set; }
        public Nullable<int> roomID { get; set; }
        public byte firstLessonDay { get; set; }
        public byte firstLessonHour { get; set; }
        public Nullable<byte> secondLessonDay { get; set; }
        public Nullable<byte> secondLessonHour { get; set; }
        public Nullable<byte> thirdLessonDay { get; set; }
        public Nullable<byte> thirdLessonHour { get; set; }
        public Nullable<byte> fourthLessonDay { get; set; }
        public Nullable<byte> fourthLessonHour { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual Course Course { get; set; }
        public virtual Room Room { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
