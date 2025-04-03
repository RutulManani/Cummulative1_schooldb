using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Cummulative1_schooldb.Models;
using MySql.Data.MySqlClient;

namespace Cummulative1_schooldb.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly SchoolDbContext Cummulative1_schooldb = new SchoolDbContext();

        // GET: TeacherPage/List
        public ActionResult List(string SearchKey)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        // GET: TeacherPage/Show/{id}
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

        // GET: TeacherPage/SearchByHireDate
        public ActionResult SearchByHireDate()
        {
            return View();
        }

        // POST: TeacherPage/SearchByHireDate
        [HttpPost]
        public ActionResult SearchByHireDate(DateTime minDate, DateTime maxDate)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.SearchByHireDate(minDate, maxDate);
            return View("List", Teachers);
        }

        // GET: TeacherPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST: TeacherPage/Add
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal Salary)
        {
            // Create new Teacher object from form data
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.EmployeeNumber = EmployeeNumber;
            NewTeacher.HireDate = HireDate;
            NewTeacher.Salary = Salary;


            // Client-side validation for immediate feedback
            if (string.IsNullOrWhiteSpace(TeacherFname) || string.IsNullOrWhiteSpace(TeacherLname))
            {
                ViewBag.ErrorMessage = "Teacher first and last name are required";
                return View("New", NewTeacher);
            }

            if (HireDate > DateTime.Now)
            {
                ViewBag.ErrorMessage = "Hire date cannot be in the future";
                return View("New", NewTeacher);
            }

            if (string.IsNullOrWhiteSpace(EmployeeNumber) || !System.Text.RegularExpressions.Regex.IsMatch(EmployeeNumber, @"^T\d+$"))
            {
                ViewBag.ErrorMessage = "Employee number must be 'T' followed by digits (e.g., T123)";
                return View("New", NewTeacher);
            }

            TeacherAPIController controller = new TeacherAPIController();
            var result = controller.AddTeacher(NewTeacher);

            if (result is System.Web.Http.Results.OkResult)
            {
                // On success, redirect to the list page
                return RedirectToAction("List");
            }
            else if (result is System.Web.Http.Results.BadRequestErrorMessageResult badRequest)
            {
                // If there was a bad request, show the error message
                ViewBag.ErrorMessage = badRequest.Message;
                return View("New", NewTeacher);
            }
            else if (result is System.Web.Http.Results.NotFoundResult)
            {
                ViewBag.ErrorMessage = "Teacher not found";
                return View("New", NewTeacher);
            }

            // Default error case
            ViewBag.ErrorMessage = "Failed to add teacher";
            return View("New", NewTeacher);
        }


        // GET: TeacherPage/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
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

        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherAPIController controller = new TeacherAPIController();
            var result = controller.DeleteTeacher(id);

            if (result is System.Web.Http.Results.OkResult)
            {
                return RedirectToAction("List");
            }

            return View("Error", new ErrorViewModel { Message = "Failed to delete teacher" });
        }
    }
}
