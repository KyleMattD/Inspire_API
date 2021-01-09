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

namespace Inspire_API.Controllers
{
    public class AccessController : ApiController
    {
        private string _key = "GuessThePasswordToThisSiteAndGetACookieFromMeOrIGetACookieFromYou";


        private IFGAfricaEntities db = new IFGAfricaEntities();


        /*[HttpPost]
        [Route("api/Learner/Register")]
        public IHttpActionResult Register(UserRegister user)
        {
            var finduser = db.Users.Where(x => x.User_Email == user.Email).FirstOrDefault();
            if (finduser == null)
            {
                user.Password = encrypt(user.Password);
                User addUser = new User();
                addUser.User_Email = user.Email;
                addUser.User_Password = user.Password;


                Learner faddLearner = new Learner();
                faddLearner.Learner_ID = user.Id;
                faddLearner.Learner_Name = user.Name;
                faddLearner.Learner_Surname = user.Surname;
                faddLearner.Learner_DOB = user.DOB;
                faddLearner.Learner_Address = user.Adress;
                faddLearner.Learner_DocFile = user.DocFile;

                db.Users.Add(addUser);
                db.SaveChanges();

                var id = db.Users.Where(x => x.User_Email == user.Email).FirstOrDefault().User_ID;

                faddLearner.User_ID = id;

                db.Learner.Add(faddUser);
                db.SaveChanges();


                return Ok("1 row affected");
            }
            else
            {
                return Unauthorized();
            }

        }*/


        [HttpPost]
        [Route("api/UserExists")]
        public IHttpActionResult UserExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.Learner.Where(x => x.Learner_ID == someText).FirstOrDefault();
            var ID2 = db.Users.Where(x => x.User_Email == someText).FirstOrDefault();

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
        public IHttpActionResult InsuranceRegister(Register User)
        {
            var finduser = db.Users.Where(x => x.User_Email == User.User_Email).FirstOrDefault();
            if (finduser == null)
            {
                User.User_Password = encrypt(User.User_Password);
                User addUser = new User();
                addUser.User_Email = User.User_Email;
                addUser.User_Password =User.User_Password;

                User faddUser = new User();
                faddUser.Learner_Name = User.User_Name;
                faddUser.Learner_Surname = User.User_Surname;
                faddUser.Learner_DOB = User.User_DOB;
                faddUser.Learner_Address = User.Address;
                faddUser.Learner_Grade = User.Current_Grade;
                faddUser.Learner_School = User.Current_School;


                db.Users.Add(addUser);
                db.SaveChanges();

                var id = db.Users.Where(x => x.User_Email == User.User_Email).FirstOrDefault().User_ID;

                faddUser.User_ID = id;

                db.Insurance_Provider.Add(faddUser);
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
        public IHttpActionResult InsuranceUserExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.User.Where(x => x.IP_ID.ToString() == someText).FirstOrDefault();
            var ID2 = db.Users.Where(x => x.User_Email == someText).FirstOrDefault();

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
        public IHttpActionResult Login(User user)
        {
            var finduser = db.Users.Where(x => x.User_Email == user.User_Email).FirstOrDefault();

            var encPass = encrypt(user.User_Password);
            if (finduser != null && finduser.User_Password == encPass)
            {

                var claims = new[]
                {
                   new Claim(ClaimTypes.Name,finduser.User_ID.ToString())
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
            var query = from user in db.Users
                        where user.User_Email == ID
                        select new
                        {
                            User_ID = user.User_ID,
                            User_Email = user.User_Email
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
                    mail.Body = "Please click on the following link to reset your AgriLog password. " + link;


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
                var User = db.Users.Where(x => x.User_ID == userID).FirstOrDefault();
                var password = newPass.Content.ReadAsStringAsync().Result;
                password = encrypt(password);
                User.User_Password = password;

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
