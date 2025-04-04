using System;
using System.Collections.Generic;
using System.Web.Http.Results;
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
            // Client-side validation matching the API validation
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

            if (result is OkResult)
            {
                return RedirectToAction("List");
            }
            else if (result is BadRequestErrorMessageResult badRequest)
            {
                ViewBag.ErrorMessage = badRequest.Message;
                return View("New", NewTeacher);
            }

            ViewBag.ErrorMessage = "Failed to add teacher";
            return View("New", NewTeacher);
        }

        // GET: TeacherPage/DeleteConfirm/{id}
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

        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherAPIController apiController = new TeacherAPIController();
            var result = apiController.DeleteTeacher(id);

            if (result is System.Web.Http.Results.OkResult)
            {
                return RedirectToAction("List");
            }

            return View("Error", new ErrorViewModel { Message = "Failed to delete teacher" });
        }
    }
}
