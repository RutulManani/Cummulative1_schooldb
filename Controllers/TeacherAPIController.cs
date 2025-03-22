using System;
using System.Collections.Generic;
using System.Web.Http;
using Cummulative1_schooldb.Models;
using MySql.Data.MySqlClient;

namespace Cummulative1_schooldb.Controllers
{
    /// <summary>
    /// API Controller that provides teacher-related data from the database.
    /// </summary>
    public class TeacherAPIController : ApiController
    {
        private readonly SchoolDbContext Cummulative1_schooldb = new SchoolDbContext();
        /// <summary>
        /// Retrieves a list of teachers based on a search key, or all teachers if no key is provided.
        /// </summary>
        /// <param name="SearchKey">Optional search keyword for filtering teachers.</param>
        /// <returns>A collection of teacher records.</returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            // Open a connection to the MySQL database
            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            // Execute SQL query to find matching teachers
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");

            cmd.Prepare();

            // Process results and populate list
            MySqlDataReader ResultSet = cmd.ExecuteReader();
            List<Teacher> Teachers = new List<Teacher>();

      
            while (ResultSet.Read())
            {
                // Clumn information
                int TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                string TeacherFname = ResultSet["teacherFname"].ToString();
                string TeacherLname = ResultSet["teacherLname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                // Teacher object and its properties
                Teacher NewTeacher = new Teacher
                {
                    TeacherId = TeacherId,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                };

                Teachers.Add(NewTeacher);
            }

            Conn.Close();
            return Teachers;
        }



        /// <summary>
        /// Retrieves detailed information of a specific teacher based on the provided teacher ID.
        /// </summary>
        /// <param name="id">The unique identifier of the teacher.</param>
        /// <returns>A Teacher object containing the teacher's details.</returns>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // Create a new Teacher
            Teacher NewTeacher = new Teacher();

            // Create a connection to MySQL database
            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            // Define an SQL command to fetch teacher details using the given ID
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();


            while (ResultSet.Read())
            {
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                NewTeacher.TeacherFname = ResultSet["teacherFname"].ToString();
                NewTeacher.TeacherLname = ResultSet["teacherLname"].ToString();
                NewTeacher.EmployeeNumber = ResultSet["employeenumber"].ToString();
                NewTeacher.HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                NewTeacher.Salary = Convert.ToDecimal(ResultSet["salary"]);
            }
            ResultSet.Close();


            // Close the database
            Conn.Close();
            return NewTeacher;
        }
    }
}