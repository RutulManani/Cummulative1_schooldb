using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Cummulative1_schooldb.Models;
using MySql.Data.MySqlClient;

namespace Cummulative1_schooldb.Controllers
{
    /// <summary>
    /// TeacherPageController handles the actions related to displaying teacher data.
    /// </summary>
    public class TeacherPageController : Controller
    {
        /// <summary>
        /// Retrieves a list of teachers based on the search key provided.
        /// </summary>
        /// <param name="SearchKey">The keyword used to filter the teacher list.</param>
        /// <returns>A view displaying a list of teachers that match the search criteria.</returns>
        // GET: TeacherPage/List
        public ActionResult List(string SearchKey)
        {
            TeacherAPIController controller = new TeacherAPIController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }
        /// <summary>
        /// Retrieves detailed information of a specific teacher based on the provided teacher ID.
        /// </summary>
        /// <param name="id">The unique identifier of the teacher.</param>
        /// <returns>A view displaying the teacher's details.</returns>
        // GET: TeacherPage/Show/{id}
        public ActionResult Show(int id)
        {
            TeacherAPIController controller = new TeacherAPIController();
            Teacher NewTeacher = controller.FindTeacher(id);

            return View(NewTeacher);
        }
    }
}