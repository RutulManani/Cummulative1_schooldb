using System;
using System.Collections.Generic;
using System.Web.Http;
using Cummulative1_schooldb.Models;
using MySql.Data.MySqlClient;

namespace Cummulative1_schooldb.Controllers
{
    public class TeacherAPIController : ApiController
    {
        private readonly SchoolDbContext Cummulative1_schooldb = new SchoolDbContext();

        /// <summary>
        /// Returns a list of all teachers in the system, optionally filtered by a search key
        /// </summary>
        /// <param name="SearchKey">Optional search term to filter by name, hire date, or salary</param>
        /// <returns>
        /// A list of Teacher objects containing teacher details
        /// Example request: GET /api/TeacherData/ListTeachers
        /// Example request with search: GET /api/TeacherData/ListTeachers/smith
        /// </returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");

            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();
            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                Teacher NewTeacher = new Teacher
                {
                    TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                    TeacherFname = ResultSet["teacherFname"].ToString(),
                    TeacherLname = ResultSet["teacherLname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                };
                Teachers.Add(NewTeacher);
            }

            Conn.Close();
            return Teachers;
        }

        /// <summary>
        /// Finds a teacher by their ID in the database
        /// </summary>
        /// <param name="id">The teacher ID to search for</param>
        /// <returns>
        /// A Teacher object containing all teacher details
        /// Example request: GET /api/TeacherData/FindTeacher/5
        /// Returns HTTP 404 if teacher not found
        /// </returns>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public IHttpActionResult FindTeacher(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Teacher ID");
            }

            Teacher NewTeacher = new Teacher();
            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            // Get teacher info
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            if (!ResultSet.HasRows)
            {
                Conn.Close();
                return NotFound();
            }

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

            // Get courses taught by this teacher
            MySqlCommand courseCmd = Conn.CreateCommand();
            courseCmd.CommandText = "SELECT * FROM Courses WHERE teacherid = @id";
            courseCmd.Parameters.AddWithValue("@id", id);
            courseCmd.Prepare();

            MySqlDataReader CourseResultSet = courseCmd.ExecuteReader();
            NewTeacher.Courses = new List<Course>();

            while (CourseResultSet.Read())
            {
                Course NewCourse = new Course
                {
                    CourseId = Convert.ToInt32(CourseResultSet["courseid"]),
                    CourseCode = CourseResultSet["coursecode"].ToString(),
                    CourseName = CourseResultSet["coursename"].ToString(),
                    TeacherId = id,
                    StartDate = Convert.ToDateTime(CourseResultSet["startdate"]),
                    FinishDate = Convert.ToDateTime(CourseResultSet["finishdate"])
                };
                NewTeacher.Courses.Add(NewCourse);
            }

            Conn.Close();
            return Ok(NewTeacher);
        }

        /// <summary>
        /// Searches for teachers by hire date range
        /// </summary>
        /// <param name="minDate">Minimum hire date (inclusive)</param>
        /// <param name="maxDate">Maximum hire date (inclusive)</param>
        /// <returns>
        /// A list of Teacher objects hired within the date range
        /// Example request: GET /api/TeacherData/SearchByHireDate?minDate=2000-01-01&maxDate=2010-12-31
        /// </returns>
        [HttpGet]
        [Route("api/TeacherData/SearchByHireDate")]
        public IEnumerable<Teacher> SearchByHireDate(DateTime minDate, DateTime maxDate)
        {
            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE hiredate BETWEEN @minDate AND @maxDate";
            cmd.Parameters.AddWithValue("@minDate", minDate);
            cmd.Parameters.AddWithValue("@maxDate", maxDate);
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();
            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                Teacher NewTeacher = new Teacher
                {
                    TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                    TeacherFname = ResultSet["teacherFname"].ToString(),
                    TeacherLname = ResultSet["teacherLname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                };
                Teachers.Add(NewTeacher);
            }

            Conn.Close();
            return Teachers;
        }

    }
}
