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
    public class USERController : Controller
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
        public ActionResult login(string First_name,string Last_name,string Email,string Role,string Password,string Password2,
            string Organization,string Lead_user,string Service_company,   string forgot)
        {  
            string token = doAuthenticate(userName: Email, password: Password, clientID: "User");
            bool result = validateAccessToken(token);
            List<CAEMS_authenticate_User> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate(password: Password, email: Email, Service_company: Service_company); 
                response = ( List<CAEMS_authenticate_User>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "User" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Service_company"] = response[0].Service_company; 
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            Session["client"] = response[0].Organization;
                            Session["status"] = "Please change your password";
                            List<CAEMS_right_User> rightList = centralCalls.get_right_User(" where id in (select `right` from CAEMS_role_right_User where role =" + response[0].Role.ToString() + " )");
                            string strRightList = "";
                            foreach (CAEMS_right_User right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            return RedirectToAction("Change_Password", "User");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "User" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Service_company"] = response[0].Service_company;
                            Session["client"] = response[0].Organization;
                            List<CAEMS_right_User> rightList = centralCalls.get_right_User(" where id in (select `right` from CAEMS_role_right_User where role =" + response[0].Role.ToString() + " )");
                            string strRightList = "";
                            foreach (CAEMS_right_User right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            return RedirectToAction("view_Users", "User"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "User");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "User");
                } 
            }
            else {
                ActionResult xx = forgotauthenticate_User(Email: Email, Service_company: Service_company); 
                response = ( List<CAEMS_authenticate_User>)Session["response"]; 
                return RedirectToAction("Login", "User");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password, string Service_company)
        { 
            List<CAEMS_authenticate_User> response = null;
            password = Audit.GetEncodedHash(password, "doing it well");
            if (Service_company != "Select Service company - Organization" && Service_company != null)
            {
                response = centralCalls.get_authenticate_User(" where Service_Company = " + Service_company.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim() + "   and Organization = " + Service_company.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim() + " and replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            }
            else
            {
                response = centralCalls.get_authenticate_User(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            }
            Session["response"]  = response;
            return Content(string.Join( "sphinxsplit",  response ));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_User(string Email, string Service_company)
        {
            List<CAEMS_authenticate_User> response = null;
            if (Service_company != null || Service_company == "Select Service company - Organization")
            {
                //response = centralCalls.get_authenticate_Admin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "'  and Service_Company = " + Service_company);
                response = centralCalls.get_authenticate_User(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "'   and Service_Company = " + Service_company.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim() + "   and Organization = " + Service_company.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
            }
            else
            {
                response = centralCalls.get_authenticate_User(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            }
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_User(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oOrganization: response[0].Organization.ToString() ,oLead_user: response[0].Lead_user.ToString() ,oService_company: response[0].Service_company.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Organization: response[0].Organization.ToString() ,Lead_user: response[0].Lead_user.ToString() ,Service_company: response[0].Service_company.ToString() ) ; 
                        string mailSubject = "Profile password reset on (sphinxsolaries) Change And Error Management System";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                } 
            } 
            Session["response"]  = response; 
            return Content(  string.Join( "sphinxsplit",  response )  );
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
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
                    return RedirectToAction("Login", "User") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "User") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            List<CAEMS_authenticate_User> response = null; 
            string result = "Authentication failed"; 
                    string strRND11 = password;
                    byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_User(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_User(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oOrganization: response[0].Organization.ToString() ,oLead_user: response[0].Lead_user.ToString() ,oService_company: response[0].Service_company.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Organization: response[0].Organization.ToString() ,Lead_user: response[0].Lead_user.ToString() ,Service_company: response[0].Service_company.ToString() ) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }



        [AllowAnonymous]
        public ActionResult view_it_Service_Company_no_secure2(string email)
        {
            Session["response"] = centralCalls.get_Client2(" where id in (select Service_Company from CAEMS_authenticate_User where  replace(email, '@','#') = '" + email.Replace("@", "#") + "' )  ",email);
            //List<CAEMS_Client2> get_Client2(string sql, string email)
            return Content(JsonConvert.SerializeObject((List<CAEMS_Client2>)Session["response"])); 
        }

        [AllowAnonymous]
        public ActionResult new_Users()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 = centralCalls.get_role_User(" where   client is null or  client = " + Session["client"].ToString());
            ViewBag.Data1 =  centralCalls.get_Client("");
            ViewBag.Data2 =  centralCalls.get_Service_Company("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Users(string First_name,string Last_name,string Email,string Role
            ,string Password
            ,string Password2
            //, string Lead_user
            //,string Service_company 
            )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            string response =null;
            string strRND =  Audit.GenerateRandom() ;
            Password = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND, "doing it well")));
            Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND, "doing it well")));
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                ActionResult xx = add_Users(First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Password, Password2: Password2,
                    Organization: Session["client"].ToString(),
                    Lead_user: "0",
                    Service_company: Session["Service_company"].ToString(), token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccessf") > -1 ){
                    string mailSubject = "User Profile creation on (sphinxsolaries) Change And Error Management System";
                    string mailBody = "Hi <br><br>You have been successfully profiled on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                    Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                }
                return RedirectToAction("new_Users", "User");
        } 

        [AllowAnonymous]
        public ActionResult add_Users(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_authenticate_User(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Users()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            List<CAEMS_authenticate_User3> response = null; 
           ActionResult d =  view_it_Users( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
            return View((List<CAEMS_authenticate_User3>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Users(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_authenticate_User3>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_authenticate_User3>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_User3(" where Lead_User =0 and Organization = " + Session["client"].ToString());
            return  Content(JsonConvert.SerializeObject(  (List<CAEMS_authenticate_User3>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Users(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2
            //,string Organization,string Lead_user,string Service_company  
            )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 = centralCalls.get_role_User(" where   client is null or  client = " + Session["client"].ToString());
            ViewBag.Data1 =  centralCalls.get_Client("");
            ViewBag.Data2 =  centralCalls.get_Service_Company("");
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
             ViewBag.Last_name = Last_name;
             ViewBag.Email = Email;
             ViewBag.Role = Role;
             //ViewBag.Password = Password;
             //ViewBag.Password2 = Password2;
             //ViewBag.Organization = Organization;
             //ViewBag.Lead_user = Lead_user;
             //ViewBag.Service_company = Service_company;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Users(string id,string oFirst_name,string oLast_name,string oEmail,string oRole
            //,string oPassword,string oPassword2,
            //string oOrganization
                ,string oLead_user
            //,string oService_company
            ,string First_name,string Last_name,string Email,string Role
            //,string Password,string Password2,string Organization,string Lead_user,string Service_company 
            )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            string response =null;
                ActionResult xx =  update_Users(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole
                    ,oPassword:  null,oPassword2:  null,oOrganization:  null,oLead_user:  null,oService_company:  null,
                    First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,
                    Password: null,Password2: null,Organization: null,Lead_user: null,Service_company: null, 
                    token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Users", "User");
                }
                else{
                      ViewBag.First_name = First_name;
                      ViewBag.Last_name = Last_name;
                      ViewBag.Email = Email;
                      ViewBag.Role = Role;
                      //ViewBag.Password = Password;
                      //ViewBag.Password2 = Password2;
                      //ViewBag.Organization = Organization;
                      //ViewBag.Lead_user = Lead_user;
                      //ViewBag.Service_company = Service_company;
                     
                     return View();
                }
                return RedirectToAction("new_Users", "User");
        } 

        [AllowAnonymous]
        public ActionResult update_Users(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oOrganization,string oLead_user,string oService_company,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            response =  centralCalls.update_authenticate_User(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oOrganization:  oOrganization,oLead_user:  oLead_user,oService_company:  oService_company,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Change_Or_Errors()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 = centralCalls.get_Project(" where Organization   = " + Session["client"].ToString() + " ");
            ViewBag.Data1 =  centralCalls.get_Service_Company("");
            ViewBag.Data2 =  centralCalls.get_authenticate_Admin("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Change_Or_Errors(string Project,string Service_company,string User,string Change_or_error_detail,string Log_date )
        {
            //                                  (string Project,string Service_company,string User,string Change_or_error_detail,string Log_date )
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                ActionResult xx =  add_Change_Or_Errors(Project: Project,
                    //Service_company: Service_company,User: User ,
                    Change_or_error_detail: Change_or_error_detail,Log_date: Log_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Change_Or_Errors", "User");
        } 

        [AllowAnonymous]
        public ActionResult add_Change_Or_Errors(string Project,
            //string Service_company,string User,
            string Change_or_error_detail,string Log_date,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Change_Or_Error(Project: Project,
                Initiating_User_Type: "2", User: Session["userID"].ToString(),
                Change_or_error_detail: Change_or_error_detail,Log_date: Log_date);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Change_Or_Errors()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                List<CAEMS_Change_Or_Error2> response = null; 
           ActionResult d =  view_it_Change_Or_Errors( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                return View((List<CAEMS_Change_Or_Error2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Change_Or_Errors(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_Change_Or_Error2>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            {
                Session["response"] = new List<CAEMS_Change_Or_Error2>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Change_Or_Error2("    where Project in (select id from CAEMS_Project where    Organization  = " + Session["client"].ToString() + "  ) ");
            return Content(JsonConvert.SerializeObject((List<CAEMS_Change_Or_Error2>)Session["response"]));
        }

        [AllowAnonymous]
        public ActionResult edit_Change_Or_Errors(string id,string Project,string Service_company,string User,string Change_or_error_detail,string Log_date  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 = centralCalls.get_Project(" where Organization   = " + Session["client"].ToString() + " ");
            ViewBag.Data1 =  centralCalls.get_Service_Company("");
            ViewBag.Data2 =  centralCalls.get_authenticate_Admin("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Project = Project;
             ViewBag.Service_company = Service_company;
             ViewBag.User = User;
             ViewBag.Change_or_error_detail = Change_or_error_detail;
             ViewBag.Log_date = Log_date;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Change_Or_Errors(string id,string oProject,string oService_company,string oUser,string oChange_or_error_detail,string oLog_date,string Project,string Service_company,string User,string Change_or_error_detail,string Log_date )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            string response =null;
                ActionResult xx =  update_Change_Or_Errors(id:id,oProject:  oProject,oService_company:  oService_company,oUser:  oUser,oChange_or_error_detail:  oChange_or_error_detail,oLog_date:  oLog_date,Project: Project,Service_company: Service_company,User: User,Change_or_error_detail: Change_or_error_detail,Log_date: Log_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Change_Or_Errors", "User");
                }
                else{
                      ViewBag.Project = Project;
                      ViewBag.Service_company = Service_company;
                      ViewBag.User = User;
                      ViewBag.Change_or_error_detail = Change_or_error_detail;
                      ViewBag.Log_date = Log_date;
                     
                     return View();
                }
                return RedirectToAction("new_Change_Or_Errors", "User");
        } 

        [AllowAnonymous]
        public ActionResult update_Change_Or_Errors(string id, string oProject,string oService_company,string oUser,string oChange_or_error_detail,string oLog_date,string Project,string Service_company,string User,string Change_or_error_detail,string Log_date,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            response =  centralCalls.update_Change_Or_Error(id:id,oProject:  oProject
                //,oService_company:  oService_company,oUser:  oUser
                ,oChange_or_error_detail:  oChange_or_error_detail
                //,oLog_date:  oLog_date
                ,Project: Project
                //,Service_company: Service_company,User: User
                ,Change_or_error_detail: Change_or_error_detail
                //,Log_date: Log_date
                ,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Change_Or_Error_Movement()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 =  centralCalls.get_Change_Or_Error("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Change_Or_Error_Movement(string Change,string Action,string Action_user_type,string Action_user )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                ActionResult xx =  add_Change_Or_Error_Movement(Change: Change,Action: Action,Action_user_type: Action_user_type,Action_user: Action_user, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Change_Or_Error_Movement", "User");
        } 

        [AllowAnonymous]
        public ActionResult add_Change_Or_Error_Movement(string Change,string Action,string Action_user_type,string Action_user,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;
            response = centralCalls.add_new_Change_Or_Error_Movement(Change: Change, Action: Action, Action_user_type: "2", Action_user: Session["userID"].ToString());
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Change_Or_Error_Movement()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                List<CAEMS_Change_Or_Error_Movement2> response = null; 
           ActionResult d =  view_it_Change_Or_Error_Movement( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                return View((List<CAEMS_Change_Or_Error_Movement2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Change_Or_Error_Movement(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_Change_Or_Error_Movement2>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            {
                Session["response"] = new List<CAEMS_Change_Or_Error_Movement2>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Change_Or_Error_Movement2("    where Project in (select id from CAEMS_Project where    Organization = " + Session["client"].ToString() + " ) ");
            return Content(JsonConvert.SerializeObject((List<CAEMS_Change_Or_Error_Movement2>)Session["response"]));
        }

        [AllowAnonymous]
        public ActionResult edit_Change_Or_Error_Movement(string id, string ActionValue, string Project_Title, string Change2, string Change)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 = centralCalls.get_Change_Or_Error("");
            getStatus();
            ViewBag.id = id;
            ViewBag.Change2 = Change2;
            ViewBag.Change = Change;
            ViewBag.ActionValue = ActionValue;
            ViewBag.Project_Title = Project_Title; 
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Change_Or_Error_Movement(string id, string oAction, string Project_Title, string Change2, string Change, string x)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            string response =null;
               // ActionResult xx =  update_Change_Or_Error_Movement(id:id,oChange:  oChange,oAction:  oAction,oAction_user_type:  oAction_user_type,oAction_user:  oAction_user,Change: Change,Action: Action,Action_user_type: Action_user_type,Action_user: Action_user, token: Session["token"].ToString() ,role: Session["role"].ToString()  );


                ActionResult xx = update_Change_Or_Error_Movement(id: id,
                    Action: oAction,
                    Change: Change,
                    //Action_user_type: "1", 
                    //Action_user: Session["userID"].ToString(), 
                    token: Session["token"].ToString(), role: Session["role"].ToString()); 
            
            if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Change_Or_Error_Movement", "User");
                }
                else{

                    ViewBag.Project_Title = Project_Title;
                    ViewBag.Change2 = Change2;
                    ViewBag.id = id;
                    ViewBag.Action = oAction; 
                     
                     return View();
                }
                return RedirectToAction("new_Change_Or_Error_Movement", "User");
        } 

        [AllowAnonymous] 
        public ActionResult update_Change_Or_Error_Movement(string id, 
            //string oChange,
            //string oAction,
            //string oAction_user_type,
            //string oAction_user,
            string Change,
            string Action,
            //string Action_user_type,
            //string Action_user,
            string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            //response =  centralCalls.update_Change_Or_Error_Movement(id:id,oChange:  oChange,oAction:  oAction,oAction_user_type:  oAction_user_type,oAction_user:  oAction_user,Change: Change,Action: Action,Action_user_type: Action_user_type,Action_user: Action_user,andPassword: false);
            response = centralCalls.add_new_Change_Or_Error_Movement(Change: Change,
                Action: Action,
                Action_user_type: "2",
                Action_user: Session["userID"].ToString());
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_User_Roles()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
            ViewBag.Data1 = centralCalls.get_right_User("");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_User_Roles(string Rolename, string selectedRights)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                ActionResult xx = add_User_Roles(Rolename: Rolename,   token: Session["token"].ToString(), role: Session["role"].ToString(),
                    selectedRights: selectedRights, Service_company: Session["Service_company"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_User_Roles", "User");
        } 

        [AllowAnonymous]
        public ActionResult add_User_Roles(string Rolename, string token, string role, string selectedRights, string Service_company)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;
            response = centralCalls.add_new_role_User(Rolename: Rolename, Client: Session["client"].ToString(), selectedRights: selectedRights, Service_company: Service_company);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_User_Roles()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            List<CAEMS_role_User> response = null; 
           ActionResult d =  view_it_User_Roles( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
            return View((List<CAEMS_role_User>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_User_Roles(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_role_User>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_role_User>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_User(" where  client = " + Session["client"].ToString());
            return  Content(JsonConvert.SerializeObject(  (List<CAEMS_role_User>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_User_Roles(string id,string Rolename   )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Rolename = Rolename;
             //ViewBag.Client = Client;
             ViewBag.Data1 = centralCalls.get_right_User("");
             List<CAEMS_role_right_User> roleRightUserList = centralCalls.get_role_right_User(" where role = " + id);
             string rightSet = "";
             foreach (CAEMS_role_right_User roleRightUser in roleRightUserList)
             {
                 rightSet += "sphinxcol" + roleRightUser.Right + "sphinxcol";
             }

             ViewBag.rightSet = rightSet;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_User_Roles(string id, string oRolename,  string Rolename, string selectedRights)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            string response =null;
//            ActionResult xx = update_Admin_Roles(id: id, oRolename: oRolename, Rolename: Rolename, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedRights: selectedRights); 

              ActionResult xx =  update_User_Roles(id:id,oRolename:  oRolename, Rolename: Rolename,  token: Session["token"].ToString() ,role: Session["role"].ToString() , selectedRights: selectedRights ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_User_Roles", "User");
                }
                else{
                      ViewBag.Rolename = Rolename;
                     // ViewBag.Client = Client;
                     
                     return View();
                }
                return RedirectToAction("new_User_Roles", "User");
        } 

        [AllowAnonymous]
        public ActionResult update_User_Roles(string id, string oRolename, string Rolename, string token, string role, string selectedRights)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            }
            response = centralCalls.update_role_User(id: id, oRolename: oRolename, Rolename: Rolename, andPassword: false, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_User_Rights()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 =  centralCalls.get_role_User("");
            ViewBag.Data1 =  centralCalls.get_right_User("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_User_Rights(string Role,string Right )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
                ActionResult xx =  add_User_Rights(Role: Role,Right: Right, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_User_Rights", "User");
        } 

        [AllowAnonymous]
        public ActionResult add_User_Rights(string Role,string Right,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'new User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_role_right_User(Role: Role,Right: Right);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_User_Rights()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            List<CAEMS_role_right_User> response = null; 
           ActionResult d =  view_it_User_Rights( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
            return View((List<CAEMS_role_right_User>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_User_Rights(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'view User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_role_right_User>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_role_right_User>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_right_User("");
            return  Content(JsonConvert.SerializeObject(  (List<CAEMS_role_right_User>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_User_Rights(string id,string Role,string Right  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
            ViewBag.Data0 =  centralCalls.get_role_User("");
            ViewBag.Data1 =  centralCalls.get_right_User("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Role = Role;
             ViewBag.Right = Right;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_User_Rights(string id,string oRole,string oRight,string Role,string Right )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "User");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "User");
            }
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "User");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "User");
                }
            string response =null;
                ActionResult xx =  update_User_Rights(id:id,oRole:  oRole,oRight:  oRight,Role: Role,Right: Right, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "User");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_User_Rights", "User");
                }
                else{
                      ViewBag.Role = Role;
                      ViewBag.Right = Right;
                     
                     return View();
                }
                return RedirectToAction("new_User_Rights", "User");
        } 

        [AllowAnonymous]
        public ActionResult update_User_Rights(string id, string oRole,string oRight,string Role,string Right,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_User("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_User where  replace(rightName,'_',' ') = 'edit User Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            response =  centralCalls.update_role_right_User(id:id,oRole:  oRole,oRight:  oRight,Role: Role,Right: Right,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
