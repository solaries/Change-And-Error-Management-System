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
    public class ADMINController : Controller
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
        public ActionResult login(string First_name, string Last_name, string Email, string Role, string Password, string Password2, 
            string Service_company, string Leadadmin, string forgot)
        {  
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Admin");
            bool result = validateAccessToken(token);
            List<CAEMS_authenticate_Admin> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate(password: Password, email: Email, Service_company: Service_company); 
                response = ( List<CAEMS_authenticate_Admin>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Service_company"] = response[0].Service_company; 
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            Session["status"] = "Please change your password";
                            List<CAEMS_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from CAEMS_role_right_Admin where role =" + response[0].Role.ToString() + " )");
                            string strRightList = "";
                            foreach (CAEMS_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            return RedirectToAction("Change_Password", "Admin");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Service_company"] = response[0].Service_company; 
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;  
                            List<CAEMS_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from CAEMS_role_right_Admin where role =" + response[0].Role.ToString() + " )");
                            string strRightList = "";
                            foreach(CAEMS_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            return RedirectToAction("View_Users", "Admin"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Admin");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Admin");
                } 
            }
            else {
                ActionResult xx = forgotauthenticate_Admin(Email: Email, Service_company: Service_company); 
                response = ( List<CAEMS_authenticate_Admin>)Session["response"]; 
                return RedirectToAction("Login", "Admin");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password, string Service_company)
        { 
            List<CAEMS_authenticate_Admin> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            if (Service_company != "Select Service company" && Service_company!=null)
            {
                response = centralCalls.get_authenticate_Admin(" where Service_Company =" + Service_company + " and replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' "); 
            }
            else
            { 
                response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' "); 
            }
            Session["response"]  = response;
            return Content(string.Join( "sphinxsplit",  response ));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Admin(string Email, string Service_company)
        {   
            List<CAEMS_authenticate_Admin> response = null;
            if (Service_company != null) 
            {
                response = centralCalls.get_authenticate_Admin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "'  and Service_Company = " + Service_company); 
            }
            else
            {
                response =  centralCalls.get_authenticate_Admin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' " ); 
            }
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Admin0(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oService_company: response[0].Service_company.ToString() ,oLeadadmin: response[0].Leadadmin.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Service_company: response[0].Service_company.ToString() ,Leadadmin: response[0].Leadadmin.ToString() ) ;
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
        public ActionResult Register()
        {
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Register(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin)
        {  
           string response =null;
           Leadadmin = "";
                ActionResult xx = addRegister(First_name:  First_name,Last_name:  Last_name,Email:  Email,Role:  Role,Password:  Password,Password2:  Password2,Service_company:  Service_company,Leadadmin:  Leadadmin ); 
                response = ( string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccessf") == -1 ){
                        return RedirectToAction("Register", "Admin"); 
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                } 
        } 

        [AllowAnonymous]
        public ActionResult addRegister(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin)
        { 
                string response = "";
                Leadadmin = "1";
                string strRND = Audit.GenerateRandom();
                byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                response =  centralCalls.add_new_authenticate_Admin(First_name:  First_name,Last_name:  Last_name,Email:  Email,Role:  Role,Password: Encoding.ASCII.GetString(arr)  ,Password2: Encoding.ASCII.GetString(arr)  ,Service_company:  Service_company,Leadadmin:  Leadadmin);
                if(response.IndexOf("uccessf") > -1 ){
                    string mailSubject = "Admin Profile creation on (sphinxsolaries) Change And Error Management System";
                    string mailBody = "Hi <br><br>You have been successfully profiled on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                    Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                }
                Session["response"]  = response;
                return Content( response);
        }



        [AllowAnonymous]
        public ActionResult Change_Password()
        {
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
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
                    return RedirectToAction("Login", "Admin") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Admin") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            List<CAEMS_authenticate_Admin> response = null; 
            string result = "Authentication failed"; 
                    string strRND11 = password;
                    byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Admin0(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oService_company: response[0].Service_company.ToString() ,oLeadadmin: response[0].Leadadmin.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Service_company: response[0].Service_company.ToString() ,Leadadmin: response[0].Leadadmin.ToString() ) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult new_Users()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_User(" where service_company = " + Session["Service_company"].ToString());
            ViewBag.Data1 = centralCalls.get_Client(" where  not(id in (select distinct Organization from CAEMS_authenticate_User))  and service_company = " + Session["Service_company"].ToString());
            ViewBag.Data2 =  centralCalls.get_Service_Company("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Users(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                //string strRND = Audit.GenerateRandom();
            string strRND =  Audit.GenerateRandom() ;





                Password=Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND , "doing it well")));
                Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND, "doing it well")));
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Users(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccessf") > -1 ){
                    string mailSubject = "User Profile creation on (sphinxsolaries) Change And Error Management System";
                    string mailBody = "Hi <br><br>You have been successfully profiled on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                    Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                }
                return RedirectToAction("new_Users", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Users(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Users' )   ").Count ==0){ 
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
            if(Session["userType"] == null)
            { 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Users' )   ").Count ==0)
            { 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
            }
            List<CAEMS_authenticate_User2> response = null; 
            ActionResult d =  view_it_Users( Session["token"].ToString() , Session["role"].ToString()  ); 
            if(Session["status"].ToString()=="You do not have access to this functionality")
            { 
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Admin");
            }
            return View((List<CAEMS_authenticate_User2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Users(string token, string role)
        {        
            Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_authenticate_User2>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_authenticate_User2>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            //Session["response"] = centralCalls.get_authenticate_User(" where Service_company=" + Session["Service_company"]);
            Session["response"] = centralCalls.get_authenticate_User2(  Session["Service_company"].ToString());
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_authenticate_User2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Users(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_User("");
            ViewBag.Data1 =  centralCalls.get_Client("");



            ViewBag.Data1 = centralCalls.get_Client(" where  not(id in (select distinct Organization from CAEMS_authenticate_User where Organization <> " + Organization + "))  and service_company = " + Session["Service_company"].ToString());

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
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Users(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oOrganization:  oOrganization,oLead_user:  oLead_user,oService_company:  oService_company,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Organization: Organization,Lead_user: Lead_user,Service_company: Service_company, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Users", "Admin");
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
                return RedirectToAction("new_Users", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Users(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oOrganization,string oLead_user,string oService_company,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Users' )   ").Count ==0){ 
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
        public ActionResult new_Clients()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_Service_Company("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Clients(string Name,string Service_company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Clients(Name: Name,Service_company: Service_company, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Clients", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Clients(string Name,string Service_company,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Clients' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Client(Name: Name,Service_company: Service_company);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Clients()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_Client> response = null; 
           ActionResult d =  view_it_Clients( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_Client>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Clients(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_Client>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Client>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Client(" where Service_company = " + Session["Service_company"].ToString());
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Client>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Clients(string id,string Name,string Service_company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
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
        public ActionResult edit_Clients(string id,string oName,/*string oService_company,*/string Name/*,string Service_company*/ , string x)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Clients' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Clients(id:id,oName:  oName/*,oService_company:  oService_company*/,Name: Name,/*Service_company: Service_company,*/ token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Clients", "Admin");
                }
                else{
                      ViewBag.Name = Name;
                      //ViewBag.Service_company = Service_company;
                     
                     return View();
                }
                return RedirectToAction("new_Clients", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Clients(string id, string oName,/*string oService_company,*/string Name,/*string Service_company,*/string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Clients' )   ").Count ==0){ 
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
            response =  centralCalls.update_Client(id:id,oName:  oName,/*oService_company:  oService_company,*/Name: Name,/*Service_company: Service_company,*/andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Project()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_Client(" where  id in (select distinct Organization from CAEMS_authenticate_User) and  Service_company = " + Session["Service_company"].ToString());
            //ViewBag.Data1 =  centralCalls.get_authenticate_User("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Project(string Project_title, string Organization)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx = add_Project(Project_title: Project_title, Organization: Organization, token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Project", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Project(string Project_title, string Organization, string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Project' )   ").Count ==0){ 
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
            response = centralCalls.add_new_Project(Project_title: Project_title, Organization: Organization);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Project()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_Project2> response = null; 
           ActionResult d =  view_it_Project( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_Project2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Project(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_Project2>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Project2>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Project2(" where Service_Company = " + Session["Service_company"].ToString());
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Project2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Project(string id, string Project_title, string Organization)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_Client(" where id in (select distinct Organization from CAEMS_authenticate_User) and Service_company = " + Session["Service_company"].ToString());
            //ViewBag.Data0 =  centralCalls.get_Service_Company("");
            //ViewBag.Data1 =  centralCalls.get_authenticate_User("");
            getStatus();
            ViewBag.id=id;
            ViewBag.Project_title = Project_title;
            ViewBag.Organization = Organization;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Project(string id, string oProject_title, string oOrganization, string Project_title, string Organization)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Project' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
            ActionResult xx = update_Project(id: id, oProject_title: oProject_title, oOrganization: oOrganization, Project_title: Project_title, Organization: Organization, token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Project", "Admin");
                }
                else{
                      ViewBag.Project_title = Project_title;
                      ViewBag.Organization = Organization;
                      ViewBag.User = User;
                     
                     return View();
                }
                return RedirectToAction("new_Project", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Project(string id, string oProject_title, string oOrganization, string Project_title, string Organization, string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Project' )   ").Count ==0){ 
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
            response = centralCalls.update_Project(id: id, oProject_title: oProject_title, oOrganization: oOrganization, Project_title: Project_title, Organization: Organization, andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Change_Or_Errors()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_Project(" where Organization in (select id from CAEMS_Client where Service_Company = " + Session["Service_company"].ToString() + ") ");
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
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Change_Or_Errors(Project: Project,
                    //Service_company: Service_company, User: User,
                    Change_or_error_detail: Change_or_error_detail,Log_date: Log_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Change_Or_Errors", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Change_Or_Errors(string Project, 
            //string Initiating_User_Type, string User, 
            string Change_or_error_detail, string Log_date, string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Errors' )   ").Count ==0){ 
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
                Initiating_User_Type: "1", User: Session["userID"].ToString(),
                Change_or_error_detail: Change_or_error_detail,Log_date: Log_date);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Change_Or_Errors()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_Change_Or_Error2> response = null; 
           ActionResult d =  view_it_Change_Or_Errors( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_Change_Or_Error2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Change_Or_Errors(string token, string role)
        {
            Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Change Or Errors' )   ").Count ==0){ 
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
            Session["response"] = centralCalls.get_Change_Or_Error2("    where Project in (select id from CAEMS_Project where    Organization in (select id from CAEMS_Client where Service_Company = " + Session["Service_company"].ToString() + ")  ) ");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Change_Or_Error2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Change_Or_Errors(string id,string Project
            //,string Service_company,string User
            ,string Change_or_error_detail
            //,string Log_date  
            )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_Project(" where Organization in (select id from CAEMS_Client where Service_Company = " + Session["Service_company"].ToString() + ") ");
            ViewBag.Data1 =  centralCalls.get_Service_Company("");
            ViewBag.Data2 =  centralCalls.get_authenticate_Admin("");
            getStatus();
            ViewBag.id=id;
            ViewBag.Project = Project;
            // ViewBag.Service_company = Service_company;
            // ViewBag.User = User;
            ViewBag.Change_or_error_detail = Change_or_error_detail;
             //ViewBag.Log_date = Log_date;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Change_Or_Errors(string id,string oProject,
            //string oService_company,string oUser,
            string oChange_or_error_detail
            //,string oLog_date
            ,string Project
           // ,string Service_company,string User
            ,string Change_or_error_detail
            //,string Log_date 
            )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Change_Or_Errors(id:id,oProject:  oProject,
                    //oService_company:  oService_company,oUser:  oUser,
                    oChange_or_error_detail:  oChange_or_error_detail,
                    //oLog_date:  oLog_date,
                    Project: Project,
                    //Service_company: Service_company,User: User,
                    Change_or_error_detail: Change_or_error_detail,
                    //Log_date: Log_date, 
                    token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Change_Or_Errors", "Admin");
                }
                else{
                      ViewBag.Project = Project;
                      //ViewBag.Service_company = Service_company;
                      //ViewBag.User = User;
                      ViewBag.Change_or_error_detail = Change_or_error_detail;
                      //ViewBag.Log_date = Log_date;
                     
                     return View();
                }
                return RedirectToAction("new_Change_Or_Errors", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Change_Or_Errors(string id, string oProject,
            //string oService_company,string oUser,
            string oChange_or_error_detail,
            //string oLog_date,
            string Project,
            //string Service_company,string User,
            string Change_or_error_detail,
            //string Log_date,
            string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Errors' )   ").Count ==0){ 
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
                ,oChange_or_error_detail:  oChange_or_error_detail,
                //oLog_date:  oLog_date,
                Project: Project
                //,Service_company: Service_company,User: User
                ,Change_or_error_detail: Change_or_error_detail,
                //Log_date: Log_date,
                andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Service_Company()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Service Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Service_Company(string Company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Service Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Service_Company(Company: Company, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Service_Company", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Service_Company(string Company,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Service Company' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Service_Company(Company: Company);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Service_Company()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Service Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_Service_Company> response = null; 
           ActionResult d =  view_it_Service_Company( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_Service_Company>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Service_Company(string token, string role)
        {
            Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Service Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_Service_Company>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_Service_Company>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Service_Company(" order by Company ");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Service_Company>)Session["response"] ));
        }


        [AllowAnonymous]
        public ActionResult view_it_Service_Company_no_secure(  string email)
        {
            Session["response"] = centralCalls.get_Service_Company(" where id in (select Service_Company from CAEMS_authenticate_Admin where  replace(email, '@','#') = '" + email.Replace("@", "#") + "' )     order by Company");

            return Content(JsonConvert.SerializeObject((List<CAEMS_Service_Company>)Session["response"]));
            //return Content(JsonConvert.SerializeObject(cscList));
        }
        [AllowAnonymous]
        public ActionResult edit_Service_Company(string id,string Company  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if (centralCalls.get_role_right_Admin("  where role =  " + Session["role"] + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Service Company' )").Count == 0)
            { 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
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
        public ActionResult edit_Service_Company(string id,string oCompany,string Company )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Service Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Service_Company(id:id,oCompany:  oCompany,Company: Company, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Service_Company", "Admin");
                }
                else{
                      ViewBag.Company = Company;
                     
                     return View();
                }
                return RedirectToAction("new_Service_Company", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Service_Company(string id, string oCompany,string Company,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Service Company' )   ").Count ==0){ 
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
            response =  centralCalls.update_Service_Company(id:id,oCompany:  oCompany,Company: Company,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Change_Or_Error_Movement()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
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
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Change_Or_Error_Movement(Change: Change,Action: Action,Action_user_type: Action_user_type,Action_user: Action_user, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Change_Or_Error_Movement", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Change_Or_Error_Movement(string Change,string Action,string Action_user_type,string Action_user,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Change Or Error Movement' )   ").Count ==0){ 
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
            response = centralCalls.add_new_Change_Or_Error_Movement(Change: Change, Action: Action, Action_user_type: "1", Action_user: Session["userID"].ToString());
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Change_Or_Error_Movement()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_Change_Or_Error_Movement2> response = null; 
           ActionResult d =  view_it_Change_Or_Error_Movement( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_Change_Or_Error_Movement2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Change_Or_Error_Movement(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Change Or Error Movement' )   ").Count ==0){ 
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
            Session["response"] = centralCalls.get_Change_Or_Error_Movement2("    where Project in (select id from CAEMS_Project where    Organization in (select id from CAEMS_Client where Service_Company = " + Session["Service_company"].ToString() + ")  ) ");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_Change_Or_Error_Movement2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Change_Or_Error_Movement(string id, string ActionValue, string Project_Title, string Change2, string Change)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_Change_Or_Error("");
            getStatus();
            ViewBag.id=id;
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
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
            ActionResult xx = update_Change_Or_Error_Movement(id: id, 
                Action: oAction, 
                Change: Change,
                //Action_user_type: "1", 
                //Action_user: Session["userID"].ToString(), 
                token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Change_Or_Error_Movement", "Admin");
                }
                else{
                    ViewBag.Project_Title = Project_Title;
                      ViewBag.Change2 = Change2; 
                      ViewBag.id = id; 
                      ViewBag.Action = oAction; 
                     
                     return View();
                }
                return RedirectToAction("new_Change_Or_Error_Movement", "Admin");
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
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Change Or Error Movement' )   ").Count ==0){ 
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
                Action_user_type: "1", 
                Action_user: Session["userID"].ToString());

            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Admin_Roles()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();

            ViewBag.Data1 = centralCalls.get_right_Admin("");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Admin_Roles(string Rolename, string selectedRights)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx = add_Admin_Roles(Rolename: Rolename, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedRights: selectedRights, Service_company:Session["Service_company"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Admin_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Admin_Roles(string Rolename, string token, string role, string selectedRights, string Service_company)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Roles' )   ").Count ==0){ 
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
            response = centralCalls.add_new_role_Admin(Rolename: Rolename, selectedRights: selectedRights, Service_company: Service_company);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Admin_Roles()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_role_Admin> response = null; 
           ActionResult d =  view_it_Admin_Roles( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_role_Admin>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Admin_Roles(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_role_Admin>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_role_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_Admin(" where  service_company = " + Session["Service_company"].ToString());
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_role_Admin>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Admin_Roles(string id,string Rolename  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
            ViewBag.Rolename = Rolename;
            ViewBag.Data1 = centralCalls.get_right_Admin("");
            List<CAEMS_role_right_Admin> roleRightAdminList = centralCalls.get_role_right_Admin(" where role = " + id);
            string rightSet = "";
            foreach(CAEMS_role_right_Admin roleRightAdmin in roleRightAdminList)
            {
                rightSet += "sphinxcol" + roleRightAdmin.Right + "sphinxcol";
            }

            ViewBag.rightSet = rightSet;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Admin_Roles(string id, string oRolename, string Rolename, string selectedRights)
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                string response =null;
                ActionResult xx = update_Admin_Roles(id: id, oRolename: oRolename, Rolename: Rolename, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedRights: selectedRights ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Admin_Roles", "Admin");
                }
                else{
                      ViewBag.Rolename = Rolename;
                     
                     return View();
                }
                return RedirectToAction("new_Admin_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Admin_Roles(string id, string oRolename, string Rolename, string token, string role, string selectedRights)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Roles' )   ").Count ==0){ 
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
            response = centralCalls.update_role_Admin(id: id, oRolename: oRolename, Rolename: Rolename, andPassword: false, selectedRights: selectedRights );
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Admin_Role_To_Rights()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            ViewBag.Data1 =  centralCalls.get_right_Admin("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Admin_Role_To_Rights(string Role,string Right )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Admin_Role_To_Rights(Role: Role,Right: Right, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Admin_Role_To_Rights", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Admin_Role_To_Rights(string Role,string Right,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Admin Role To Rights' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_role_right_Admin(Role: Role,Right: Right);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Admin_Role_To_Rights()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<CAEMS_role_right_Admin> response = null; 
           ActionResult d =  view_it_Admin_Role_To_Rights( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_role_right_Admin>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Admin_Role_To_Rights(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_role_right_Admin>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_role_right_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_right_Admin("");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_role_right_Admin>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Admin_Role_To_Rights(string id,string Role,string Right  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            ViewBag.Data1 =  centralCalls.get_right_Admin("");
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
        public ActionResult edit_Admin_Role_To_Rights(string id,string oRole,string oRight,string Role,string Right )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Role To Rights' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Admin_Role_To_Rights(id:id,oRole:  oRole,oRight:  oRight,Role: Role,Right: Right, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Admin_Role_To_Rights", "Admin");
                }
                else{
                      ViewBag.Role = Role;
                      ViewBag.Right = Right;
                     
                     return View();
                }
                return RedirectToAction("new_Admin_Role_To_Rights", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Admin_Role_To_Rights(string id, string oRole,string oRight,string Role,string Right,string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Admin Role To Rights' )   ").Count ==0){ 
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
            response =  centralCalls.update_role_right_Admin(id:id,oRole:  oRole,oRight:  oRight,Role: Role,Right: Right,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Administrators()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_role_Admin(" where  service_company is null or service_company = " + Session["Service_company"].ToString());
            ViewBag.Data1 =  centralCalls.get_Service_Company("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin )
        {
            Leadadmin = "0";
            Service_company = Session["Service_company"].ToString();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            string response =null;
            //string strRND = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well")));
            string strRND = Audit.GenerateRandom() ;
                Password=Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND , "doing it well"))) ;
                Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND, "doing it well")));
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Administrators(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Service_company: Service_company,Leadadmin: Leadadmin, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccessf") > -1 ){
                    string mailSubject = "Admin Profile creation on (sphinxsolaries) Change And Error Management System";
                    string mailBody = "Hi <br><br>You have been successfully profiled on the (sphinxsolaries) Change And Error Management System platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                    Audit.SendMail(Email, mailSubject, mailBody, "add profile"); 
                }
                return RedirectToAction("new_Administrators", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin,string token, string role)
        { 
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_authenticate_Admin(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Service_company: Service_company,Leadadmin: Leadadmin);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Administrators()
        {
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                List<CAEMS_authenticate_Admin2> response = null; 
           ActionResult d =  view_it_Administrators( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<CAEMS_authenticate_Admin2>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Administrators(string token, string role)
        {
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'view Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<CAEMS_authenticate_Admin2>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<CAEMS_authenticate_Admin2>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            //Session["response"] = centralCalls.get_authenticate_Admin(" where service_company = " + Session["Service_company"].ToString() + " and  LeadAdmin =0  order by first_name, last_name, email");
            Session["response"] = centralCalls.get_authenticate_Admin2("  " + Session["Service_company"].ToString() + " ");
            return Content(JsonConvert.SerializeObject(  (List<CAEMS_authenticate_Admin2>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Administrators(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin  )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_role_Admin(" where  service_company is null or service_company = " + Session["Service_company"].ToString());
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
        public ActionResult edit_Administrators(string id,string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oService_company,string oLeadadmin,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin )
        {  
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Administrators(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole, First_name: First_name,Last_name: Last_name,Email: Email,Role: Role, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Administrators", "Admin");
                }
                else{
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
                return RedirectToAction("new_Administrators", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Administrators(string id, string oFirst_name,string oLast_name,string oEmail,string oRole, string First_name,string Last_name,string Email,string Role, string token, string role)
        { 
            string response = null;  
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from CAEMS_right_Admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
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
            response =  centralCalls.update_authenticate_Admin(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole, First_name: First_name,Last_name: Last_name,Email: Email,Role: Role, andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
