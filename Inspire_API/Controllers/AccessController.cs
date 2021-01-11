using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Inspire_API.Models;
using System.Web.Configuration;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using MassTransit.Contracts.Conductor;
using System.Data.Linq;

namespace Inspire_API.Controllers
{
    public class AccessController : ApiController
    {
        private string _key = "GuessThePasswordToThisSiteAndGetACookieFromMeOrIGetACookieFromYou";


        private InspireEntities db = new InspireEntities();

        [HttpPost]
        [Route("api/UserExists")]
        public IHttpActionResult UserExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.People.Where(x => x.Person_Name == someText).FirstOrDefault();
            var ID2 = db.People.Where(x => x.Person_Email == someText).FirstOrDefault();

            if (ID1 != null || ID2 != null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/User/Register")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult PersonRegister(Register person)
        {
            var finduser = db.People.Where(x => x.Person_Email == person.Email).FirstOrDefault();
            if (finduser == null)
            {
                person.Password = encrypt(person.Password);
                Person addUser = new Person();
                addUser.Person_Email = person.Email;
                addUser.Person_Password =person.Password;

                Person paddUser = new Person();
                paddUser.Person_Name = person.Name;
                paddUser.Person_Surname = person.Surname;
                paddUser.Person_Level = person.Level;
                paddUser.Person_School = person.School;
                paddUser.DocFile = person.DocFile;
                paddUser.Person_ID = person.Centre_ID;

                Person_Course addCourse = new Person_Course();
                addCourse.Person_ID = person.Course_ID;

                PersonSubject addSubject = new PersonSubject();
                addSubject.Person_ID = person.Subject_ID;

                db.PersonSubjects.Add(addSubject);
                db.Person_Course.Add(addCourse);
                db.People.Add(addUser);
                db.SaveChanges();

                var id = db.People.Where(x => x.Person_Email == person.Email).FirstOrDefault().Person_ID;

                paddUser.Person_ID = id;
                db.SaveChanges();

                return Ok("1 row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "lol nope");
            }
        }


        [HttpPost]
        [Route("api/UserExists")]
        public IHttpActionResult PersonExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.People.Where(x => x.Person_ID.ToString() == someText).FirstOrDefault();
            var ID2 = db.People.Where(x => x.Person_Email == someText).FirstOrDefault();

            if (ID1 != null || ID2 != null)
            {
                return Unauthorized();
            } 
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        [Route("api/User/Login")]
        public IHttpActionResult Login(Person person)
        {
            var finduser = db.People.Where(x => x.Person_Email == person.Person_Email).FirstOrDefault();

            var encPass = encrypt(person.Person_Password);
            if (finduser != null && finduser.Person_Password == encPass)
            {

                var claims = new[]
                {
                   new Claim(ClaimTypes.Name,finduser.Person_ID.ToString())
                };
                var keytoReturn = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

                var Credentials = new SigningCredentials(keytoReturn, SecurityAlgorithms.HmacSha512Signature);
                var descriptorToken = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = Credentials
                };
                var Handler = new JwtSecurityTokenHandler();

                var userToken = Handler.CreateToken(descriptorToken);
                return Ok
                (new
                {
                    Token = Handler.WriteToken(userToken)
                }
                );

            }
            return Unauthorized();
        }


        //===============================================================encryption of password=========================================================================================
        private string encrypt(string Pass)
        {
            var toEncrypt = Encoding.UTF8.GetBytes(Pass);
            using (SHA512 shaM = new SHA512Managed())
            {
                var hash = shaM.ComputeHash(toEncrypt);
                var hashedpass = new System.Text.StringBuilder(128);
                foreach (var b in hash)
                {
                    hashedpass.Append(b.ToString("X2"));
                }
                return hashedpass.ToString();
            }
        }


        //===============================================================================Forgot and set new password====================================================================
        [HttpPost]
        [Route("api/submitforgot")]
        public IHttpActionResult submitForgot(HttpRequestMessage email)
        {
            string ID = email.Content.ReadAsStringAsync().Result;
            var query = from Person in db.People
                        where Person.Person_Email == ID
                        select new
                        {
                            User_ID = Person.Person_ID,
                            User_Email = Person.Person_Email
                        };

            try
            {
                var result = query.ToList().FirstOrDefault();

                if (result != null)
                {

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    SmtpServer.EnableSsl = true;

                    mail.From = new MailAddress("kyledrotsky@gmail.com");
                    mail.To.Add(result.User_Email);
                    mail.Subject = "Password reset";
                    mail.Body = "Please click on the following link to reset your AgriLog password. " + Url;


                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                    return Ok();

                }
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, e);
            }

            return Ok();
        }

        [HttpPost]
        [Route("api/newPass/{userID}")]
        public IHttpActionResult newPass(int userID, HttpRequestMessage newPass)
        {
            try
            {
                var User = db.People.Where(x => x.Person_ID == userID).FirstOrDefault();
                var password = newPass.Content.ReadAsStringAsync().Result;
                password = encrypt(password);
                User.Person_Password = password;

                db.SaveChanges();


            }
            catch (Exception e)
            {

                return BadRequest();
            }
            return Ok();
        }

    }
}
