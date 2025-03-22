using System;
using System.Collections.Generic;

namespace Cummulative1_schooldb.Models
{
    /// <summary>
    /// Represents a teacher within the school system.
    /// Contains properties for teacher details and associated courses.
    /// </summary>
    public class Teacher
    {
        // Properties of the Teacher entity
        public int TeacherId;
        public string TeacherFname;
        public string TeacherLname;
        public string EmployeeNumber;
        public DateTime HireDate;
        public decimal Salary;
        public List<Course> Courses;

    }
}