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
    }
}
