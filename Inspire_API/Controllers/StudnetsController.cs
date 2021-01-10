using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNetCore.Cors;

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
            var query = 


            List<dynamic> Course = new List<dynamic>();
            try
            {
                Course = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");

            }
            if (Course.Count > 0)
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
                var query = 

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
                toUpdate = db.Farms.Where(f => f.Farm_ID == id).FirstOrDefault();
                toUpdate.Farm_Name = putfarm.Farm_Name;
                toUpdate.Farm_Size = putfarm.Farm_Size;
                toUpdate.Farm_Email = putfarm.Farm_Email;
                toUpdate.Farm_ContactNumber = putfarm.Farm_ContactNumber;
                toUpdate.Farm_Address = putfarm.Farm_Address;
                toUpdate.Farm_Type_ID = putfarm.Farm_Type_ID;
                toUpdate.Province_ID = putfarm.Province_ID;
                toUpdate.Is_Active = putfarm.Is_Active;
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
        public IHttpActionResult postCourse(int userID, Course newCourse)
        {
            
            if (newCourse != null)
            {
                try
                {
                    newCourse.Person_ID = 4;
                    db.Course.Add(newCourse);
                    db.SaveChanges();

                    var addedCourseID = newCourse.Course_ID;

                    var query = 

                    var fUserID = query.FirstOrDefault().Person_ID;

                    newEntry.Course_ID = addedCourseID;
                    newEntry.User_Type_ID = 1;

                    db.PersonCourse.Add(newEntry);
                    db.SaveChanges();

                    var auditQuery = 

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Farms.Remove(newCourse);
                    try
                    {
                        db.PersonCourse(newEntry);
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

            if (newSubject != null)
            {
                try
                {
                    newSubject.Person_ID = 4;
                    db.Course.Add(newSubject);
                    db.SaveChanges();

                    var addedSubjectID = newSubject.Subject_ID;

                    var query =

                    var fUserID = query.FirstOrDefault().Person_ID;

                    newEntry.Subject_ID = addedSubjectID;
                    newEntry.User_Type_ID = 2;

                    db.PersonSubject.Add(newEntry);
                    db.SaveChanges();

                    var auditQuery =

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Farms.Remove(newSubject);
                    try
                    {
                        db.PersonCourse(newEntry);
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
                var Subject = db.Subject.Where(f => f.CourseID == id).FirstOrDefault();
                var Subject = db.Person.Where(x => x.PersonID == CourseID).FirstOrDefault();
                Subject.Is_Active = true;
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

        [Route("api/Farm/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deletePerson(int id)
        {
            try
            {
                var Person = db.Person.Where(f => f.PersonID == id).FirstOrDefault();
                Person.Is_Active = true;
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

    }
}
