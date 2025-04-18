using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using System.Web.Mvc;
using Cummulative1_schooldb.Models;

namespace Cummulative1_schooldb.Controllers
{
    /// <summary>
    /// MVC Controller for handling teacher-related views and actions
    /// </summary>
    public class TeacherPageController : Controller
    {
        /// <summary>
        /// Displays a paginated list of teachers, optionally filtered by search key.
        /// </summary>
        /// <param name="SearchKey">Optional search term to filter teacher names</param>
        /// <returns>View containing list of teachers</returns>
        /// <example>GET /TeacherPage/List?SearchKey=smith</example>
        public ActionResult List(string SearchKey)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        /// <summary>
        /// Displays detailed information about a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display</param>
        /// <returns>
        /// View with teacher details if found, otherwise error view
        /// </returns>
        /// <example>GET /TeacherPage/Show/5</example>
        public ActionResult Show(int id)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel { Message = "Invalid Teacher ID" });
            }

            TeacherAPIController controller = new TeacherAPIController();
            var result = controller.FindTeacher(id);

            if (result is System.Web.Http.Results.NotFoundResult)
            {
                return View("Error", new ErrorViewModel { Message = "Teacher not found" });
            }

            var okResult = result as System.Web.Http.Results.OkNegotiatedContentResult<Teacher>;
            if (okResult != null)
            {
                return View(okResult.Content);
            }

            return View("Error", new ErrorViewModel { Message = "An error occurred" });
        }


        /// <summary>
        /// Displays a list of teachers with optional advanced filtering
        /// </summary>
        /// <param name="searchKey">Optional search term for teacher names or details</param>
        /// <param name="minDate">Optional minimum hire date filter</param>
        /// <param name="maxDate">Optional maximum hire date filter</param>
        /// <returns>
        /// A View containing filtered teachers list
        /// Example URL: /TeacherPage/ListAdvanced?searchKey=smith&minDate=2005-01-01&maxDate=2015-12-31
        /// </returns>

        public ActionResult ListAdvanced(string searchKey, DateTime? minDate, DateTime? maxDate)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.ListTeachersAdvanced(searchKey, minDate, maxDate);
            return View("List", Teachers);
        }

        /// <summary>
        /// Displays the form for creating a new teacher.
        /// </summary>
        /// <returns>Empty teacher creation form</returns>
        /// <example>GET /TeacherPage/New</example>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Processes submission of new teacher data.
        /// </summary>
        /// <param name="TeacherFname">First name of the teacher</param>
        /// <param name="TeacherLname">Last name of the teacher</param>
        /// <param name="EmployeeNumber">Employee number (format: TXXX)</param>
        /// <param name="HireDate">Date when teacher was hired</param>
        /// <param name="Salary">Teacher's salary</param>
        /// <returns>
        /// Redirects to teacher list on success, 
        /// returns form with errors if validation fails
        /// </returns>
        /// <example>POST /TeacherPage/Create</example>
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal Salary)
        {
            if (string.IsNullOrWhiteSpace(TeacherFname) || string.IsNullOrWhiteSpace(TeacherLname))
            {
                ViewBag.ErrorMessage = "Teacher first and last name are required";
                return View("New", new Teacher
                {
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            if (HireDate > DateTime.Now)
            {
                ViewBag.ErrorMessage = "Hire date cannot be in the future";
                return View("New", new Teacher
                {
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            if (string.IsNullOrWhiteSpace(EmployeeNumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(EmployeeNumber, @"^T[0-9]{3}$"))
            {
                ViewBag.ErrorMessage = "Employee number must be 'T' followed by digits (e.g., T123)";
                return View("New", new Teacher
                {
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            TeacherAPIController apiController = new TeacherAPIController();
            Teacher NewTeacher = new Teacher
            {
                TeacherFname = TeacherFname,
                TeacherLname = TeacherLname,
                EmployeeNumber = EmployeeNumber,
                HireDate = HireDate,
                Salary = Salary
            };

            var result = apiController.AddTeacher(NewTeacher);

            if (result is System.Web.Http.Results.OkResult ||
                result is System.Web.Http.Results.OkNegotiatedContentResult<string>)
            {
                return RedirectToAction("List");
            }

            var badRequest = result as System.Web.Http.Results.BadRequestErrorMessageResult;
            if (badRequest != null)
            {
                ViewBag.ErrorMessage = badRequest.Message;
                return View("New", NewTeacher);
            }

            ViewBag.ErrorMessage = "Failed to add teacher";
            return View("New", NewTeacher);
        }

        /// <summary>
        /// Displays confirmation page before deleting a teacher.
        /// </summary>
        /// <param name="id">ID of teacher to delete</param>
        /// <returns>
        /// Confirmation view if teacher exists, 
        /// error view if not found
        /// </returns>
        /// <example>GET /TeacherPage/DeleteConfirm/5</example>
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel { Message = "Invalid Teacher ID" });
            }

            TeacherAPIController apiController = new TeacherAPIController();
            var result = apiController.FindTeacher(id);

            if (result is System.Web.Http.Results.OkNegotiatedContentResult<Teacher> okResult)
            {
                return View(okResult.Content);
            }

            return View("Error", new ErrorViewModel { Message = "Teacher not found" });
        }

        /// <summary>
        /// Processes teacher deletion request.
        /// </summary>
        /// <param name="id">ID of teacher to delete</param>
        /// <returns>
        /// Redirects to teacher list on success,
        /// error view if deletion fails
        /// </returns>
        /// <example>POST /TeacherPage/Delete/5</example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherAPIController apiController = new TeacherAPIController();
            var result = apiController.DeleteTeacher(id);

            if (result is System.Web.Http.Results.OkResult ||
                result is System.Web.Http.Results.OkNegotiatedContentResult<string>)
            {
                return RedirectToAction("List");
            }

            return View("Error", new ErrorViewModel { Message = "Failed to delete teacher" });
        }


        /// <summary>
        /// Displays form to edit an existing teacher's information.
        /// </summary>
        /// <param name="id">ID of teacher to edit</param>
        /// <returns>
        /// Edit form with teacher data if found,
        /// error view if teacher doesn't exist
        /// </returns>
        /// <example>GET /TeacherPage/Edit/5</example>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel { Message = "Invalid Teacher ID" });
            }

            TeacherAPIController apiController = new TeacherAPIController();
            var result = apiController.FindTeacher(id);

            if (result is System.Web.Http.Results.NotFoundResult)
            {
                return View("Error", new ErrorViewModel { Message = "Teacher not found" });
            }

            var okResult = result as System.Web.Http.Results.OkNegotiatedContentResult<Teacher>;
            if (okResult != null)
            {
                return View(okResult.Content);
            }

            return View("Error", new ErrorViewModel { Message = "An error occurred" });
        }

        /// <summary>
        /// Processes submission of updated teacher information.
        /// </summary>
        /// <param name="id">ID of teacher being updated</param>
        /// <param name="TeacherFname">Updated first name</param>
        /// <param name="TeacherLname">Updated last name</param>
        /// <param name="HireDate">Updated hire date</param>
        /// <param name="Salary">Updated salary</param>
        /// <returns>
        /// Redirects to teacher details on success,
        /// returns form with errors if validation fails
        /// </returns>
        /// <example>POST /TeacherPage/Edit/5</example>
        [HttpPost]
        public ActionResult Edit(int id, string TeacherFname, string TeacherLname, DateTime HireDate, decimal Salary)
        {
            // Initiative 5: Error Handling on Update when Teacher Name is empty (Client)
            if (string.IsNullOrWhiteSpace(TeacherFname) || string.IsNullOrWhiteSpace(TeacherLname))
            {
                ViewBag.ErrorMessage = "Teacher first and last name are required";
                return View(new Teacher
                {
                    TeacherId = id,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            // Initiative 6: Error Handling on Update when Hire Date is in future (Client)
            if (HireDate > DateTime.Now)
            {
                ViewBag.ErrorMessage = "Hire date cannot be in the future";
                return View(new Teacher
                {
                    TeacherId = id,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            // Initiative 7: Error Handling on Update when Salary is negative (Client)
            if (Salary < 0)
            {
                ViewBag.ErrorMessage = "Salary cannot be negative";
                return View(new Teacher
                {
                    TeacherId = id,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    HireDate = HireDate,
                    Salary = Salary
                });
            }

            TeacherAPIController apiController = new TeacherAPIController();
            Teacher UpdatedTeacher = new Teacher
            {
                TeacherId = id,
                TeacherFname = TeacherFname,
                TeacherLname = TeacherLname,
                HireDate = HireDate,
                Salary = Salary
            };

            var result = apiController.UpdateTeacher(id, UpdatedTeacher);

            if (result is OkNegotiatedContentResult<Teacher> okResult)
            {
                // Success - redirect to the Show page with the updated teacher
                return RedirectToAction("Show", new { id = id });
            }
            else if (result is BadRequestErrorMessageResult badRequest)
            {
                ViewBag.ErrorMessage = badRequest.Message;
                return View(UpdatedTeacher);
            }
            else if (result is NotFoundResult)
            {
                return View("Error", new ErrorViewModel { Message = "Teacher not found" });
            }

            return View("Error", new ErrorViewModel { Message = "Failed to update teacher" });
        }
    }
}
