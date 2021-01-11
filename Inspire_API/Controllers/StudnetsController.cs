using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Inspire_API.Models;
using System.Web.Http.Cors;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Web.Configuration;
using System.Web.Http.Description;

namespace Inspire_API.Controllers
{
    public class StudnetsController : ApiController
    {
        private InspireEntities db = new InspireEntities();

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Farm/{UserID}")]
        [HttpGet]
        public IHttpActionResult getCourse(int UserID, Course Course)
        {
            var query = from p in db.People
                        join pc in db.Person_Course on p.Person_ID equals pc.Person_ID
                        where p.Person_ID == UserID
                        select new
                        {

                        };


            List < dynamic > Courses = new List<dynamic>();
            try
            {
                Courses = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");

            }
            if (Courses.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.CourseList = Course;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified user has no course");
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Subject/{id}")]
        public IHttpActionResult getSubject(int id)
        {
            dynamic toReturn = new ExpandoObject();
            try
            {
                var query = from s in db.Subjects
                            join ps in db.PersonSubjects on s.Subject_ID equals ps.Subject_ID
                            where ps.Subject_ID == id
                            select new
                            { 
                            
                            };


                toReturn = query.ToList().FirstOrDefault();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error");
            }
            if (toReturn != null)
            {
                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No Subject was found with that ID");
            }
        }

        [Route("api/Course/put/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putCourse(int id, Course putCourse)
        {
            Course toUpdate = new Course();
            try
            {
                toUpdate = db.Courses.Where(f => f.Course_ID == id).FirstOrDefault();
                toUpdate.Course_Name = putCourse.Course_Name;
                toUpdate.Course_Capacity = putCourse.Course_Capacity;
                db.SaveChanges();

                var auditQuery = 
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "1 Row Affected");
            }
            catch (Exception e)
            {
                if (!CourseExists(id))
                {

                    return Content(HttpStatusCode.BadRequest, "Course doesnt exist");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Edit Failed");
                }
            }
        }

        private bool CourseExists(int id)
        {
            throw new NotImplementedException();
        }

        [Route("api/Course/put/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putPerson(int id, Person putPerson)
        {
            Person toUpdate = new Person();
            try
            {
                toUpdate = db.People.Where(f => f.Person_ID == id).FirstOrDefault();
                toUpdate.Person_Name = putPerson.Person_Name;
                toUpdate.Person_Surname = putPerson.Person_Surname;
                toUpdate.Person_Email = putPerson.Person_Email;
                toUpdate.Person_Level = putPerson.Person_Level;
                toUpdate.Person_School = putPerson.Person_School;
                toUpdate.Centre_ID = putPerson.Centre_ID;
                toUpdate.Person_Course = putPerson.Person_Course;
                toUpdate.PersonSubject = putPerson.PersonSubject;
                db.SaveChanges();

                var auditQuery =
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "1 Row Affected");
            }
            catch (Exception e)
            {
                if (!CourseExists(id))
                {

                    return Content(HttpStatusCode.BadRequest, "Course doesnt exist");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Edit Failed");
                }
            }
        }

        //=======================================AddFarm==================================================

        // POST: api/Farms
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("api/Course/add/{UserID}")]
        public IHttpActionResult postCourse(int userID,  Course newCourse)
        {
            Course newEntry = new Course();
            if (newCourse != null)
            {
                try
                {
                    db.Courses.Add(newCourse);

                    db.SaveChanges();
                    var addedCourseID = newCourse.Course_ID;

                    var query = from c in db.Courses
                                join pc in db.Person_Course on
                                c.Course_ID equals pc.Course_ID
                                where pc.Course_ID == userID
                                select new
                                {

                                };

                    newEntry.Course_ID = addedCourseID;
                    newEntry.Course_ID = 2;

                    db.Courses.Add(newEntry);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Courses.Remove(newCourse);
                    try
                    {
                        db.Courses.Add(newEntry);
                    }
                    catch (Exception x)
                    {

                        return Content(HttpStatusCode.BadRequest, "POST failed");
                    }

                    return Content(HttpStatusCode.BadRequest, "POST failed");
                }
                return Content(HttpStatusCode.OK, "1 Row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("api/Subject/add/{UserID}")]
        public IHttpActionResult postSubject(int userID, Subject newSubject)
        {
            Person_Course newEntry = new Person_Course();
            if (newSubject != null)
            {
                try
                {
                    db.Subjects.Add(newSubject);
                    db.SaveChanges();

                    var addedSubjectID = newSubject.Subject_ID;

                    var query = from s in db.Subjects
                                join ps in db.PersonSubjects on s.Subject_ID equals ps.Subject_ID
                                where s.Subject_ID == userID
                                select new 
                                { 

                                };


                    var UserID = query.FirstOrDefault();

                    newEntry.Course_ID = addedSubjectID;
                    newEntry.Person_Course_ID= 2;

                    db.Person_Course.Add(newEntry);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Subjects.Remove(newSubject);
                    try
                    {
                        db.Person_Course(newEntry);
                    }
                    catch (Exception x)
                    {

                        return Content(HttpStatusCode.BadRequest, "POST failed");
                    }

                    return Content(HttpStatusCode.BadRequest, "POST failed");
                }
                return Content(HttpStatusCode.OK, "1 Row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }


        [Route("api/Farm/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deleteSubject(int id)
        {
            try
            {
                var Subject = db.Subjects.Where(f => f.Subject_ID == id).FirstOrDefault();
                var subject = db.Subjects.Where(x => x.PersonSubject.Subject_ID == id).FirstOrDefault();
                db.SaveChanges();

                var auditQuery = 
                
                db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

        [Route("api/Person/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deletePerson(int id)
        {
            try
            {
                var Person = db.People.Where(f => f.Person_ID == id).FirstOrDefault();
                Person.Is_Active = false;
                db.SaveChanges();

                var auditQuery = from p in db.People
                                 join pc in db.Person_Course on p.Person_ID equals pc.Person_ID
                                 join ps in db.PersonSubjects on p.Person_ID equals ps.Person_ID
                                 where p.Person_ID == id
                                 select new
                                 {

                                 };

                db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

    }
}
