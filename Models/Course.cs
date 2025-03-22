using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cummulative1_schooldb.Models
{
    /// <summary>
    /// Represents a course offered within the school system.
    /// </summary>
    public class Course
    {
        // Properties of the Course entity
        public int CourseId;
        public string CourseCode;
        public int TeacherId;
        public DateTime StartDate;
        public DateTime FinishDate;
        public string CourseName;
    }
}