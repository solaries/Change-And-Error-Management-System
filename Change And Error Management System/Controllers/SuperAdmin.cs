using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography; 
using System.Text;
using sphinxsolaries.Caems.Data;
using sphinxsolaries.Caems.Data.Models;
using sphinxsolaries.Caems.Models;  
using sphinxsolaries.Caems.BusinessLogic;  
using System.Net;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace sphinxsolaries.Caems.Controllers
{ 
    [Authorize]
    public class SUPERADMINController : Controller
    {  


          private void getStatus(bool clearStatus =true)
        { 
            if (Session["status"] != null)
            {
                if (Session["status"].ToString().Trim().Length > 0)
                {
                    ViewBag.status = Session["status"].ToString();
                    if (clearStatus)
                    {
                         Session["status"] = "";
                    }
                }
            }
            if (Session["down"] != null)
            {
                if (Session["down"].ToString().Trim().Length > 0)
                {
                    ViewBag.down = Session["down"].ToString();
                    Session["down"] = ""; 
                }
            }
        }
        private bool validateAccessToken(string token)
        {
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/api/GoodToken");
             var data = Encoding.ASCII.GetBytes("");
             newHttpRequest.Method = "GET";
             newHttpRequest.Headers.Add("Authorization", "Bearer " + token); 
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
             }
             catch (Exception ex)
             {
                 return false;
             } 
             return true;
        } 
        private string doAuthenticate(string userName, string password, string clientID)
        {
             string result = ""; 
             string dataToSend = "&username=" + HttpUtility.UrlEncode(userName) + "&password=" + HttpUtility.UrlEncode(password) + "&clientid=" + HttpUtility.UrlEncode(clientID) + "&grant_type=password";
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/token");
             var data = Encoding.ASCII.GetBytes(dataToSend);
             newHttpRequest.Method = "POST"; 
             newHttpRequest.ContentType = "application/x-www-form-urlencoded"; 
             newHttpRequest.ContentLength = data.Length;
             using (var streamProcess = newHttpRequest.GetRequestStream())
             {
                 streamProcess.Write(data, 0, data.Length);
             }
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
                 dynamic passString = JsonConvert.DeserializeObject<dynamic>(responseString);
                 result = (string)passString.access_token; 
             }
             catch (Exception d)
             { 
             }
             return result;
        }

        [AllowAnonymous]
        public ActionResult login()
        {
            getStatus();
            Session.Clear(); 
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult login(string First_name,string Last_name,string Email,string Password,string Password2,   string forgot)
        {  
            string token = doAuthenticate(userName: Email, password: Password, clientID: "SuperAdmin");
            bool result = validateAccessToken(token);
            List<CAEMS_authenticate_SuperAdmin> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                //return Content(Password + " plus " + Email);

                response = ( List<CAEMS_authenticate_SuperAdmin>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "SuperAdmin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "SuperAdmin");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "SuperAdmin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            return RedirectToAction("view_Administrators", "SuperAdmin"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "SuperAdmin");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "SuperAdmin");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_SuperAdmin(   Email: Email ); 
                response = ( List<CAEMS_authenticate_SuperAdmin>)Session["response"]; 
                return RedirectToAction("Login", "SuperAdmin");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            List<CAEMS_authenticate_SuperAdmin> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<CAEMS_authenticate_SuperAdmin>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_SuperAdmin(string Email )
        {   
            List<CAEMS_authenticate_SuperAdmin> response = null; 
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_SuperAdmin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr)) ;
                        string mailSubject = "Profile password reset on (sphinxsolaries) Change And Error Management System";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                } 
            } 
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject(response));
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            ViewBag.first_name=Session["first_name"];
            ViewBag.last_name=Session["last_name"];
            ViewBag.email=Session["email"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Change_Password(string email,string password,string npassword )
        {  
                ActionResult xx = updatePassword( password:  password,npassword:  npassword , email: email ); 
                Session["status"] = (string)Session["response"];
                if(Session["response"].ToString().IndexOf("update successful") > -1)
                {
                    Session["status"] = "Password Changed Successfully";
                    return RedirectToAction("Login", "SuperAdmin") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "SuperAdmin") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            List<CAEMS_authenticate_SuperAdmin> response = null; 
            string result = "Authentication failed"; 
                    string strRND11 = password;
                    byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_SuperAdmin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2)) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult view_Administrators()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
                List<CAEMS_authenticate_Admin2> response = null; 
           ActionResult d =  view_it_Administrators( Session["token"].ToString() );
           return View((List<CAEMS_authenticate_Admin2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Administrators(string token)
        {
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            {
                Session["response"] = new List<CAEMS_authenticate_Admin2>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_Admin2("");
            return Content(JsonConvert.SerializeObject((List<CAEMS_authenticate_Admin2>)Session["response"]));
        }

        [AllowAnonymous]
        public ActionResult edit_Administrators(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            ViewBag.Data1 =  centralCalls.get_Service_Company("");
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
             ViewBag.Last_name = Last_name;
             ViewBag.Email = Email;
             ViewBag.Role = Role;
             ViewBag.Password = Password;
             ViewBag.Password2 = Password2;
             ViewBag.Service_company = Service_company;
             ViewBag.Leadadmin = Leadadmin;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Administrators(string id,string oFirst_name,string oLast_name,string oEmail,string oRole, string First_name,string Last_name,string Email,string Role ,string x )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Administrators(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole, First_name: First_name,Last_name: Last_name,Email: Email,Role: Role, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Administrators", "SuperAdmin");
                }
                else{
                      ViewBag.First_name = First_name;
                      ViewBag.Last_name = Last_name;
                      ViewBag.Email = Email;
                      ViewBag.Role = Role;
                      //ViewBag.Password = Password;
                      //ViewBag.Password2 = Password2;
                      //ViewBag.Service_company = Service_company;
                      //ViewBag.Leadadmin = Leadadmin;
                     
                     return View();
                }
                return RedirectToAction("new_Administrators", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Administrators(string id, string oFirst_name,string oLast_name,string oEmail,string oRole, string First_name,string Last_name,string Email,string Role, string token)
        { 
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_authenticate_Admin(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole, First_name: First_name,Last_name: Last_name,Email: Email,Role: Role, andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Users()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
                List<CAEMS_authenticate_User2> response = null; 
           ActionResult d =  view_it_Users( Session["token"].ToString() );
           return View((List<CAEMS_authenticate_User2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Users(string token)
        {
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            {
                Session["response"] = new List<CAEMS_authenticate_User2>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_User2("");
            return Content(JsonConvert.SerializeObject((List<CAEMS_authenticate_User2>)Session["response"]));
        }

        [AllowAnonymous]
        public ActionResult edit_Users(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_role_User("");
            ViewBag.Data1 =  centralCalls.get_Client("");
            ViewBag.Data2 =  centralCalls.get_Service_Company("");
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
             ViewBag.Last_name = Last_name;
             ViewBag.Email = Email;
             ViewBag.Role = Role;
             ViewBag.Password = Password;
             ViewBag.Password2 = Password2;
             ViewBag.Organization = Organization;
             ViewBag.Lead_user = Lead_user;
             ViewBag.Service_company = Service_company;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Users(string id,string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oOrganization,string oLead_user,string oService_company,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Users(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oOrganization:  oOrganization,oLead_user:  oLead_user,oService_company:  oService_company,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Users", "SuperAdmin");
                }
                else{
                      ViewBag.First_name = First_name;
                      ViewBag.Last_name = Last_name;
                      ViewBag.Email = Email;
                      ViewBag.Role = Role;
                      ViewBag.Password = Password;
                      ViewBag.Password2 = Password2;
                      ViewBag.Organization = Organization;
                      ViewBag.Lead_user = Lead_user;
                      ViewBag.Service_company = Service_company;
                     
                     return View();
                }
                return RedirectToAction("new_Users", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Users(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oOrganization,string oLead_user,string oService_company,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company,string token)
        { 
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_authenticate_User(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oOrganization:  oOrganization,oLead_user:  oLead_user,oService_company:  oService_company,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Organizations()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            List<CAEMS_Client2> response = null; 
           ActionResult d =  view_it_Organizations( Session["token"].ToString() );
           return View((List<CAEMS_Client2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Organizations(string token)
        {
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Client2>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Client2("","");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Client2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Organizations(string id,string Name,string Service_company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_Service_Company("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Name = Name;
             ViewBag.Service_company = Service_company;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Organizations(string id,string oName,string oService_company,string Name,string Service_company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Organizations(id:id,oName:  oName,oService_company:  oService_company,Name: Name,Service_company: Service_company, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Organizations", "SuperAdmin");
                }
                else{
                      ViewBag.Name = Name;
                      ViewBag.Service_company = Service_company;
                     
                     return View();
                }
                return RedirectToAction("new_Organizations", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Organizations(string id, string oName,string oService_company,string Name,string Service_company,string token)
        { 
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Client(id:id,oName:  oName,/*oService_company:  oService_company,*/Name: Name,/*Service_company: Service_company,*/andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Service_Providers()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            List<CAEMS_Service_Company> response = null; 
           ActionResult d =  view_it_Service_Providers( Session["token"].ToString() ); 
            return View(Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Service_Providers(string token)
        {
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Service_Company>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Service_Company("");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Service_Company>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Service_Providers(string id,string Company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Company = Company;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Service_Providers(string id,string oCompany,string Company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Service_Providers(id:id,oCompany:  oCompany,Company: Company, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Service_Providers", "SuperAdmin");
                }
                else{
                      ViewBag.Company = Company;
                     
                     return View();
                }
                return RedirectToAction("new_Service_Providers", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Service_Providers(string id, string oCompany,string Company,string token)
        { 
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Service_Company(id:id,oCompany:  oCompany,Company: Company,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Projects()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            List<CAEMS_Project3> response = null; 
           ActionResult d =  view_it_Projects( Session["token"].ToString() );
           return View((List<CAEMS_Project3>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Projects(string token)
        {
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Project3>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Project3("");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Project3>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Projects(string id,string Project_title,string Service_company,string User  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_Service_Company("");
            ViewBag.Data1 =  centralCalls.get_authenticate_User("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Project_title = Project_title;
             ViewBag.Service_company = Service_company;
             ViewBag.User = User;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Projects(string id, string oProject_title, string oOrganization, string Project_title, string Organization)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Projects(id:id,oProject_title:  oProject_title,  oOrganization:  oOrganization,Project_title: Project_title,Organization: Organization, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Projects", "SuperAdmin");
                }
                else{
                      ViewBag.Project_title = Project_title;
                      ViewBag.Organization = Organization;
                     
                     return View();
                }
                return RedirectToAction("new_Projects", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Projects(string id, string oProject_title, string oOrganization, string Project_title, string Organization, string token)
        { 
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;
            response = centralCalls.update_Project(id: id, oProject_title: oProject_title, oOrganization: oOrganization, Project_title: Project_title, Organization: Organization, andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
