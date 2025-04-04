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

        /// <summary>
        /// Adds a new teacher to the system with validation checks
        /// </summary>
        /// <param name="NewTeacher">Teacher object containing the teacher's information</param>
        /// <returns>
        /// - Teacher first and last name are required
        /// - Hire date cannot be in the future
        /// - Employee number must be 'T' followed by digits (e.g., T123)
        /// - Employee number already exists
        /// Example request: POST /api/TeacherData/AddTeacher
        /// Example request body: 
        /// {
        ///     "TeacherFname": "John",
        ///     "TeacherLname": "Doe",
        ///     "EmployeeNumber": "T123",
        ///     "HireDate": "2023-01-15",
        ///     "Salary": 55000.00
        /// }
        /// </returns>
        /// 
        [HttpPost]
        [Route("api/TeacherData/AddTeacher")]
        public IHttpActionResult AddTeacher([FromBody] Teacher NewTeacher)
        {
            // Initiative 1: Error Handling on Add when the Teacher Name is empty
            if (string.IsNullOrWhiteSpace(NewTeacher.TeacherFname) ||
                string.IsNullOrWhiteSpace(NewTeacher.TeacherLname))
            {
                return BadRequest("Teacher first and last name are required");
            }

            // Initiative 2: Error Handling on Add when the Teacher Hire Date is in the future
            if (NewTeacher.HireDate > DateTime.Now)
            {
                return BadRequest("Hire date cannot be in the future");
            }

            // Initiative 3: Error Handling on Add when the Employee Number is not “T” followed by digits
            if (string.IsNullOrWhiteSpace(NewTeacher.EmployeeNumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(NewTeacher.EmployeeNumber, @"^T[0-9]{3}$"))
            {
                return BadRequest("Employee number must be 'T' followed by digits (e.g., T123)");
            }

            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            // Initiative 4: Error Handling on Add when the Employee Number is already taken by a different Teacher
            MySqlCommand checkCmd = Conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @empnum";
            checkCmd.Parameters.AddWithValue("@empnum", NewTeacher.EmployeeNumber);

            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
            {
                Conn.Close();
                return BadRequest("Employee number already exists");
            }

            // Insert the new teacher
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                             "VALUES (@fname, @lname, @empnum, @hiredate, @salary)";

            cmd.Parameters.AddWithValue("@fname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@lname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@empnum", NewTeacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@hiredate", NewTeacher.HireDate);
            cmd.Parameters.AddWithValue("@salary", NewTeacher.Salary);

            cmd.ExecuteNonQuery();
            Conn.Close();
            return Ok();
        }

        /// <summary>
        /// Deletes a teacher from the system by their ID
        /// </summary>
        /// <param name="id">The ID of the teacher to delete</param>
        /// <returns>
        /// HTTP 200 OK if deletion was successful
        /// HTTP 404 Not Found if teacher with specified ID doesn't exist
        /// HTTP 400 Bad Request if ID is invalid (less than or equal to 0)
        /// Note: This will first unassign any courses taught by this teacher
        /// Example request: DELETE /api/TeacherData/DeleteTeacher/5
        /// </returns>

        [HttpDelete]
        [Route("api/TeacherData/DeleteTeacher/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            if (id <= 0) return BadRequest("Invalid Teacher ID");

            MySqlConnection Conn = Cummulative1_schooldb.AccessDatabase();
            Conn.Open();

            // First unassign any courses from this teacher
            MySqlCommand unassignCmd = Conn.CreateCommand();
            unassignCmd.CommandText = "UPDATE courses SET teacherid = NULL WHERE teacherid = @id";
            unassignCmd.Parameters.AddWithValue("@id", id);
            unassignCmd.ExecuteNonQuery();

            // Then delete the teacher
            MySqlCommand deleteCmd = Conn.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
            deleteCmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = deleteCmd.ExecuteNonQuery();
            Conn.Close();

            return rowsAffected > 0 ? Ok() : (IHttpActionResult)NotFound();
        }
    }
}
