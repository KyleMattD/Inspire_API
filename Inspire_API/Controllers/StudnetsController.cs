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
using System.IO;
using System.Net.Http.Headers;
using System.Web;

namespace Inspire_API.Controllers
{
    public class StudnetsController : ApiController
    {
        private InspireEntities db = new InspireEntities();

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Course/{UserID}")]
        [HttpGet]
        public IHttpActionResult getCourse(int UserID, Course Course)
        {
            var query = from p in db.People
                        join pc in db.Person_Course on p.Person_ID equals pc.Person_ID
                        join c in db.Courses on pc.Course_ID equals c.Course_ID
                        where p.Person_ID == UserID
                        select new
                        {
                            Course_ID = pc.Course_ID,
                            Person_ID = p.Person_ID,
                            Name = c.Course_Name,
                            Capacity = c.Course_Capacity, 
                            Grade = p.Person_Level
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

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/User/{UserID}")]
        [HttpGet]
        public IHttpActionResult getLearner(int UserID, Person People)
        {
            var query = from p in db.People
                        join c in db.Centres on p.Centre_ID equals c.Centre_ID
                        where p.Person_ID == UserID
                        select new
                        {
                            Person_ID = p.Person_ID, 
                            Centre_ID = p.Centre_ID,
                            Centre_Name = c.Centre_Name,
                            Capacity = c.Centre_Capacity, 
                            Name = p.Person_Name, 
                            Surname = p.Person_Surname, 
                            School = p.Person_School, 
                            Grade = p.Person_Level, 
                        };


            List<dynamic> Person = new List<dynamic>();
            try
            {
                Person = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");

            }
            if (Person.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.PersonList = People;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "There is no specified user.");
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Subjects/{id}")]
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
                                Subject_ID = s.Subject_ID, 
                                Name = s.Subject_Name,
                                Person_ID = ps.Person_ID
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

        [Route("api/Course/update/{id}")]
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

        [Route("api/Subject/update/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putSubject(int id, Subject putSubject)
        {
            Subject toUpdate = new Subject();
            try
            {
                toUpdate = db.Subjects.Where(f => f.Subject_ID == id).FirstOrDefault();
                toUpdate.Subject_Name= putSubject.Subject_Name;
                db.SaveChanges();

                var auditQuery =
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "1 Row Affected");
            }
            catch (Exception e)
            {
                if (!CourseExists(id))
                {

                    return Content(HttpStatusCode.BadRequest, "Subject doesnt exist");
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

        [Route("api/Person/update/{id}")]
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
                                    Course_ID = c.Course_ID, 
                                    Capacity = c.Course_Capacity, 
                                    Name = c.Course_Name
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
        [Route("api/Subjects/add/{UserID}")]
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
                                    Subject_ID = s.Subject_ID,
                                    Name = s.Subject_Name,
                                    Grade = ps.Person_Grade
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
                        db.Person_Course.Add(newEntry);
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


        [Route("api/Subject/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deleteSubject(int id)
        {
            try
            {
                var Subject = db.Subjects.Where(f => f.Subject_ID == id).FirstOrDefault();
                var subject = db.Subjects.Where(x => x.PersonSubject.Subject_ID == id).FirstOrDefault();
                db.SaveChanges();

                var auditQuery = from s in db.Subjects
                                 join ps in db.PersonSubjects on s.Subject_ID equals ps.Subject_ID
                                 where s.Subject_ID == id
                                 select new
                                 {
                                     Subject_ID = s.Subject_ID,
                                     Name = s.Subject_Name,
                                     Grade = ps.Person_Grade
                                 };

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
                                     Person_ID = p.Person_ID
                                 };

                db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

        [Route("api/Course/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deleteCourse(int id)
        {
            try
            {
                var Person = db.People.Where(f => f.Person_ID == id).FirstOrDefault();
                Person.Is_Active = false;
                db.SaveChanges();

                var auditQuery  = from c in db.Courses
                                             join pc in db.Person_Course on
                                             c.Course_ID equals pc.Course_ID
                                             where pc.Course_ID == id
                                             select new
                                             {
                                                 Course_ID = c.Course_ID,
                                                 Capacity = c.Course_Capacity,
                                                 Name = c.Course_Name
                                             };

                db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

        [HttpPost]
        [Route("AddFileDetails")]
        public IHttpActionResult AddFile()
        {
            string result = "";
            try
            {
                InspireEntities objEntity = new InspireEntities();
                Person objFile = new Person();
                string fileName = null;
                string imageName = null;
                var httpRequest = HttpContext.Current.Request;
                var postedImage = httpRequest.Files["ImageUpload"];
                var postedFile = httpRequest.Files["FileUpload"];
                objFile.Person_Name = httpRequest.Form["UserName"];
                if (postedImage != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedImage.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedImage.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Files/" + imageName);
                    postedImage.SaveAs(filePath);
                }
                if (postedFile != null)
                {
                    fileName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Files/" + fileName);
                    postedFile.SaveAs(filePath);
                }

                objFile.DocFile = fileName;
                objEntity.People.Add(objFile);
                int i = objEntity.SaveChanges();
                if (i > 0)
                {
                    result = "File uploaded sucessfully";
                }
                else
                {
                    result = "File uploaded faild";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("GetFile")]
        //download file api  
        public HttpResponseMessage GetFile(string docFile)
        {
            //Create HTTP Response.  
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //Set the File Path.  
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Files/") + docFile + ".docx";
            //Check whether File exists.  
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.  
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }
            //Read the File into a Byte Array.  
            byte[] bytes = File.ReadAllBytes(filePath);
            //Set the Response Content.  
            response.Content = new ByteArrayContent(bytes);
            //Set the Response Content Length.  
            response.Content.Headers.ContentLength = bytes.LongLength;
            //Set the Content Disposition Header Value and FileName.  
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = docFile + ".docx";
            //Set the File Content Type.  
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(docFile + ".docx"));
            return response;
        }
        [HttpGet]
        [Route("GetFileDetails")]
        public IHttpActionResult GetFile()
        {
            var url = HttpContext.Current.Request.Url;
            IEnumerable<Register> lstFile = new List<Register>();
            try
            {
                InspireEntities objEntity = new InspireEntities();
                lstFile = (IEnumerable<Register>)objEntity.People.Select(a => new Person
                {
                    Person_ID = a.Person_ID,
                    Person_Email = a.Person_Email,
                    Image = url.Scheme + "://" + url.Host + ":" + url.Port + "/Files/" + a.Image,
                    DocFile = a.DocFile
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(lstFile);
        }
        /*[HttpGet]
        [Route("GetImage")]
        //download Image file api  
        public HttpResponseMessage GetImage(string image)
        {
            //Create HTTP Response.  
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //Set the File Path.  
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Files/") + image + ".PNG";
            //Check whether File exists.  
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.  
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", image);
                throw new HttpResponseException(response);
            }
            //Read the File into a Byte Array.  
            byte[] bytes = File.ReadAllBytes(filePath);
            //Set the Response Content.  
            response.Content = new ByteArrayContent(bytes);
            //Set the Response Content Length.  
            response.Content.Headers.ContentLength = bytes.LongLength;
            //Set the Content Disposition Header Value and FileName.  
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = image + ".PNG";
            //Set the File Content Type.  
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(image + ".PNG"));
            return response;
        }*/
    }
}  

