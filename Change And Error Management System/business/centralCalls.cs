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
using System.Net;

namespace sphinxsolaries.Caems.BusinessLogic
{ 
    public class centralCalls 
    {  
        private static long getVal( ) {
            if (HttpContext.Current.Session["userID"] == null)
            {
                return 0;
            }
            else {
                return long.Parse(HttpContext.Current.Session["userID"].ToString());
            }
        }
        public static string add_new_authenticate_Admin(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Service_company,string Leadadmin)
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();  
            string data = "";
            try
            { 

                CAEMS_authenticate_Admin cust = new CAEMS_authenticate_Admin(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Role =  long.Parse( Role == null  || Role.Trim().Length ==0? "1" : Role)  ;
                data += ",Role : " + Role + " ___ " + cust.Role.ToString();

                 if (Leadadmin == "1")
                 {
                     response = add_new_Service_Company(Service_company);
                     List<CAEMS_Service_Company> cscList = get_Service_Company(" where company = '" + Service_company  +"' " );
                     cust.Service_company = cscList[0].Id; // long.Parse( Service_company == null ? "1" : Service_company)  ; 
                     response = "";
                 }
                 else
                 {
                     cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;

                 }

                 data += ",Service_company : " + Service_company;
                cust.Leadadmin =   Int16.Parse(Leadadmin)  ;
                 data += ",Leadadmin : " + Leadadmin;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed authenticate_Admin add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Admin";// +d.Message + "[" + d.StackTrace + "]";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_authenticate_Admin> get_authenticate_Admin(string sql)
        { 
            List<CAEMS_authenticate_Admin> response = null;
            try
            { 
                authenticate_Admin c = new authenticate_Admin(); 
                response = c.get_authenticate_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_authenticate_Admin2> get_authenticate_Admin2(string id)
        { 
            List<CAEMS_authenticate_Admin2> response = null;
            try
            { 
                authenticate_Admin c = new authenticate_Admin(); 
                response = c.get_authenticate_Admin2(id); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }



        public static string update_authenticate_Admin0(string id, string oFirst_name, string oLast_name, string oEmail, string oRole, string oPassword, string oPassword2, string oService_company, string oLeadadmin, string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Service_company, string Leadadmin, bool andPassword = true)
        {
            string response = "";
            authenticate_Admin c = new authenticate_Admin();
            string data = "";
            try
            {
                CAEMS_authenticate_Admin cust = c.get_authenticate_Admin(" where id = " + id)[0];
                cust.First_name = First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                cust.Last_name = Last_name;
                data += ",Last_name : " + oLast_name + " -> " + Last_name;
                cust.Email = Email;
                data += ",Email : " + oEmail + " -> " + Email;
                cust.Role = long.Parse(Role == null ? "1" : Role);
                data += ",Role : " + oRole + " -> " + Role;
                cust.Service_company = long.Parse(Service_company == null ? "1" : Service_company);
                data += ",Service_company : " + oService_company + " -> " + Service_company;
                cust.Leadadmin = Int16.Parse(Leadadmin);
                data += ",Leadadmin : " + oLeadadmin + " -> " + Leadadmin;
                if (andPassword)
                {
                    cust.Password = Encoding.ASCII.GetBytes(Password);
                    cust.Password2 = Encoding.ASCII.GetBytes(Password2);
                }
                response = c.update_authenticate_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving AUTHENTICATE_ADMIN";
                }
                else
                {
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating authenticate_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }

        public static string update_authenticate_Admin( string id, string oFirst_name,string oLast_name,string oEmail,string oRole, string First_name,string Last_name,string Email,string Role,  bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();   
            string data = "";
            try
            { 
                CAEMS_authenticate_Admin cust = c.get_authenticate_Admin(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + oRole + " -> " + Role;
                 //cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 //data += ",Service_company : " + oService_company + " -> " + Service_company;
                 //cust.Leadadmin =   Int16.Parse(Leadadmin)  ;
                 //data += ",Leadadmin : " + oLeadadmin + " -> " + Leadadmin;
                 //if(andPassword)
                 //{
                 //   cust.Password =  Encoding.ASCII.GetBytes( Password);
                 //   cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 //}
                response = c.update_authenticate_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving AUTHENTICATE_ADMIN";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating authenticate_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_SuperAdmin(string First_name,string Last_name,string Email,string Password,string Password2)
        { 
            string response = ""; 
            authenticate_SuperAdmin c = new authenticate_SuperAdmin();  
            string data = "";
            try
            { 

                CAEMS_authenticate_SuperAdmin cust = new CAEMS_authenticate_SuperAdmin(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_SuperAdmin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_SUPERADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed authenticate_SuperAdmin add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_SUPERADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_SuperAdmin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_authenticate_SuperAdmin> get_authenticate_SuperAdmin(string sql)
        { 
            List<CAEMS_authenticate_SuperAdmin> response = null;
            try
            { 
                authenticate_SuperAdmin c = new authenticate_SuperAdmin(); 
                response = c.get_authenticate_SuperAdmin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_SuperAdmin( string id, string oFirst_name,string oLast_name,string oEmail,string oPassword,string oPassword2,string First_name,string Last_name,string Email,string Password,string Password2, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_SuperAdmin c = new authenticate_SuperAdmin();   
            string data = "";
            try
            { 
                CAEMS_authenticate_SuperAdmin cust = c.get_authenticate_SuperAdmin(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_SuperAdmin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_SUPERADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving AUTHENTICATE_SUPERADMIN";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_SUPERADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating authenticate_SuperAdmin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_User(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company)
        { 
            string response = ""; 
            authenticate_User c = new authenticate_User();  
            string data = "";
            try
            { 

                CAEMS_authenticate_User cust = new CAEMS_authenticate_User(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + Role;
                cust.Organization =  long.Parse( Organization == null ? "1" : Organization)  ;
                 data += ",Organization : " + Organization;
                cust.Lead_user =   Int16.Parse(Lead_user)  ;
                 data += ",Lead_user : " + Lead_user;
                cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 data += ",Service_company : " + Service_company;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_USER_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed authenticate_User add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_USER_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_User";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_USER_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_authenticate_User> get_authenticate_User(string sql)
        { 
            List<CAEMS_authenticate_User> response = null;
            try
            { 
                authenticate_User c = new authenticate_User(); 
                response = c.get_authenticate_User(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_authenticate_User3> get_authenticate_User3(string sql)
        { 
            List<CAEMS_authenticate_User3> response = null;
            try
            { 
                authenticate_User c = new authenticate_User(); 
                response = c.get_authenticate_User3(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_authenticate_User2> get_authenticate_User2(string service_company_id)
        { 
            List<CAEMS_authenticate_User2> response = null;
            try
            { 
                authenticate_User c = new authenticate_User(); 
                response = c.get_authenticate_User2(service_company_id);  
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_User( string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oOrganization,string oLead_user,string oService_company,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Organization,string Lead_user,string Service_company, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_User c = new authenticate_User();   
            string data = "";
            try
            { 
                CAEMS_authenticate_User cust = c.get_authenticate_User(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + oRole + " -> " + Role;
                 //cust.Organization =  long.Parse( Organization == null ? "1" : Organization)  ;
                 //data += ",Organization : " + oOrganization + " -> " + Organization;
                 //cust.Lead_user =   Int16.Parse(Lead_user)  ;
                 //data += ",Lead_user : " + oLead_user + " -> " + Lead_user;
                 //cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 //data += ",Service_company : " + oService_company + " -> " + Service_company;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_USER_UPDATE, data, getVal(), true);
                    response = "Error saving AUTHENTICATE_USER";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_USER_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating authenticate_User";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_USER_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Change_Or_Error(string Project
            , string Initiating_User_Type, string User
            ,string Change_or_error_detail,string Log_date)
        { 
            string response = ""; 
            Change_Or_Error c = new Change_Or_Error();  
            string data = "";
            try
            { 

                CAEMS_Change_Or_Error cust = new CAEMS_Change_Or_Error(); 
                cust.Project =  long.Parse( Project == null ? "1" : Project)  ;
                data += ",Project : " + Project;
                cust.Initiating_User_Type = long.Parse(Initiating_User_Type == null ? "1" : Initiating_User_Type);
                data += ",Initiating_User_Type : " + Initiating_User_Type;
                cust.User = long.Parse(User == null ? "1" : User);
                data += ",User : " + User;
                cust.Change_or_error_detail =  Change_or_error_detail;
                data += ",Change_or_error_detail : " + Change_or_error_detail;
                cust.Log_date = System.DateTime.Now;
                response = c.add_Change_Or_Error(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CHANGE_OR_ERROR_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed Change_Or_Error add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_CHANGE_OR_ERROR_ADD, data, getVal(), true); 
                    authenticate_Admin admin = new authenticate_Admin();
                    List<CAEMS_authenticate_Admin_And_User> adminAndUserList = admin.get_Admin_And_User(Project); 
                    Project project = new Project();
                    List<CAEMS_Project> projectList = project.get_Project(" where id  = " + Project); 
                    if (adminAndUserList.Count > 0)
                    {
                        foreach(CAEMS_authenticate_Admin_And_User adminAndUser in adminAndUserList)
                        { 
                            string mailSubject = "Change or Issue logged [Project: " + projectList[0].Project_title + "] on (sphinxsolaries) Change And Error Management System";
                            string mailBody = "Hi <br><br>Please note that a change or issue '" + Change_or_error_detail + "' has  been logged for Project '" + projectList[0].Project_title + "'  on (sphinxsolaries) Change And Error Management System <br><br><br>Regards<br><br>";
                            Audit.SendMail(adminAndUser.email, mailSubject, mailBody, "add profile"); 
                        }
                    }  
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Change_Or_Error";
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_Change_Or_Error> get_Change_Or_Error(string sql)
        { 
            List<CAEMS_Change_Or_Error> response = null;
            try
            { 
                Change_Or_Error c = new Change_Or_Error(); 
                response = c.get_Change_Or_Error(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_Change_Or_Error2> get_Change_Or_Error2(string sql)
        { 
            List<CAEMS_Change_Or_Error2> response = null;
            try
            { 
                Change_Or_Error c = new Change_Or_Error(); 
                response = c.get_Change_Or_Error2(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Change_Or_Error( string id, string oProject,
            //string oService_company,string oUser,
            string oChange_or_error_detail,
            //string oLog_date,
            string Project,
            //string Service_company,string User,
            string Change_or_error_detail,
            //string Log_date, 
            bool andPassword = true) 
        { 
            string response = ""; 
            Change_Or_Error c = new Change_Or_Error();   
            string data = "";
            try
            { 
                CAEMS_Change_Or_Error cust = c.get_Change_Or_Error(" where id = " + id  )[0]; 
                cust.Project =  long.Parse( Project == null ? "1" : Project)  ;
                data += ",Project : " + oProject + " -> " + Project;
                 //cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 //data += ",Service_company : " + oService_company + " -> " + Service_company;
                 //cust.User =  long.Parse( User == null ? "1" : User)  ;
                 //data += ",User : " + oUser + " -> " + User;
                 cust.Change_or_error_detail =  Change_or_error_detail;
                 data += ",Change_or_error_detail : " + oChange_or_error_detail + " -> " + Change_or_error_detail;
                response = c.update_Change_Or_Error(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_CHANGE_OR_ERROR_UPDATE, data, getVal(), true);
                    response = "Error saving CHANGE_OR_ERROR";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CHANGE_OR_ERROR_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating Change_Or_Error";
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Change_Or_Error_Movement(string Change,string Action,string Action_user_type,string Action_user)
        { 
            string response = ""; 
            Change_Or_Error_Movement c = new Change_Or_Error_Movement();  
            string data = "";
            try
            { 

                CAEMS_Change_Or_Error_Movement cust = new CAEMS_Change_Or_Error_Movement(); 
                cust.Change =  long.Parse( Change == null ? "1" : Change)  ;
                data += ",Change : " + Change;
                cust.Action =   short.Parse(Action)   ;
                cust.Action += 1;
                data += ",Action : " + Action;
                cust.Action_user_type =   Int16.Parse(Action_user_type)  ;
                data += ",Action_user_type : " + Action_user_type;
                cust.Action_user =  long.Parse( Action_user == null ? "1" : Action_user)  ;
                data += ",Action_user : " + Action_user;
                cust.Move_Date = DateTime.Now;
                response = c.add_Change_Or_Error_Movement(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CHANGE_OR_ERROR_MOVEMENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed Change_Or_Error_Movement add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_CHANGE_OR_ERROR_MOVEMENT_ADD, data, getVal(), true);

                    Change_Or_Error changeOrError = new Change_Or_Error();
                    List<CAEMS_Change_Or_Error> changeOrErrorList = changeOrError.get_Change_Or_Error(" where id = " +  Change);


                    authenticate_Admin admin = new authenticate_Admin();
                    List<CAEMS_authenticate_Admin_And_User> adminAndUserList = admin.get_Admin_And_User(changeOrErrorList[0].Project.ToString());
                    Project project = new Project();
                    List<CAEMS_Project> projectList = project.get_Project(" where id  = " + changeOrErrorList[0].Project.ToString());
                    if (adminAndUserList.Count > 0)
                    {
                        foreach (CAEMS_authenticate_Admin_And_User adminAndUser in adminAndUserList)
                        {
                            string mailSubject = "Change or Issue movement logged [Project: " + projectList[0].Project_title + "] on (sphinxsolaries) Change And Error Management System";
                            string mailBody = "Hi <br><br>Please note that a change or issue movement has been logged for '" + changeOrErrorList[0].Change_or_error_detail + "' on the Project '" + projectList[0].Project_title + "'  on  the (sphinxsolaries) Change And Error Management System <br><br><br>Regards<br><br>";
                            Audit.SendMail(adminAndUser.email, mailSubject, mailBody, "add profile");
                        }
                    }
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Change_Or_Error_Movement";
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_MOVEMENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_Change_Or_Error_Movement> get_Change_Or_Error_Movement(string sql)
        { 
            List<CAEMS_Change_Or_Error_Movement> response = null;
            try
            { 
                Change_Or_Error_Movement c = new Change_Or_Error_Movement(); 
                response = c.get_Change_Or_Error_Movement(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_MOVEMENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_Change_Or_Error_Movement2> get_Change_Or_Error_Movement2(string sql)
        { 
            List<CAEMS_Change_Or_Error_Movement2> response = null;
            try
            { 
                Change_Or_Error_Movement c = new Change_Or_Error_Movement(); 
                response = c.get_Change_Or_Error_Movement2(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_MOVEMENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Change_Or_Error_Movement( string id, string oChange,string oAction,string oAction_user_type,string oAction_user,string Change,string Action,string Action_user_type,string Action_user, bool andPassword = true) 
        { 
            string response = ""; 
            Change_Or_Error_Movement c = new Change_Or_Error_Movement();   
            string data = "";
            try
            { 
                CAEMS_Change_Or_Error_Movement cust = c.get_Change_Or_Error_Movement(" where id = " + id  )[0]; 
                cust.Change =  long.Parse( Change == null ? "1" : Change)  ;
                data += ",Change : " + oChange + " -> " + Change;
                 cust.Action =   Int16.Parse(Action)  ;
                 data += ",Action : " + oAction + " -> " + Action;
                 cust.Action_user_type =   Int16.Parse(Action_user_type)  ;
                 data += ",Action_user_type : " + oAction_user_type + " -> " + Action_user_type;
                 cust.Action_user =  long.Parse( Action_user == null ? "1" : Action_user)  ;
                 data += ",Action_user : " + oAction_user + " -> " + Action_user;
                response = c.update_Change_Or_Error_Movement(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_CHANGE_OR_ERROR_MOVEMENT_UPDATE, data, getVal(), true);
                    response = "Error saving CHANGE_OR_ERROR_MOVEMENT";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CHANGE_OR_ERROR_MOVEMENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating Change_Or_Error_Movement";
                Audit.InsertAudit((int)eventzz.ERROR_CHANGE_OR_ERROR_MOVEMENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Client(string Name,string Service_company)
        { 
            string response = ""; 
            Client c = new Client();  
            string data = "";
            try
            { 

                CAEMS_Client cust = new CAEMS_Client(); 
                cust.Name =  Name;
                data += ",Name : " + Name;
                cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 data += ",Service_company : " + Service_company;
                response = c.add_Client(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CLIENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed Client add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_CLIENT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Client";
                Audit.InsertAudit((int)eventzz.ERROR_CLIENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_Client> get_Client(string sql)
        { 
            List<CAEMS_Client> response = null;
            try
            { 
                Client c = new Client(); 
                response = c.get_Client(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CLIENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }


        public static List<CAEMS_Client2> get_Client2(string sql, string email)
        { 
            List<CAEMS_Client2> response = null;
            try
            { 
                Client c = new Client(); 
                response = c.get_Client2(sql,email); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CLIENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
 

        public static string update_Client( string id, string oName,/*string oService_company,*/string Name,/*string Service_company,*/ bool andPassword = true) 
        { 
            string response = ""; 
            Client c = new Client();   
            string data = "";
            try
            { 
                CAEMS_Client cust = c.get_Client(" where id = " + id  )[0]; 
                cust.Name =  Name;
                data += ",Name : " + oName + " -> " + Name;
                 //cust.Service_company =  long.Parse( Service_company == null ? "1" : Service_company)  ;
                 //data += ",Service_company : " + oService_company + " -> " + Service_company;
                response = c.update_Client(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_CLIENT_UPDATE, data, getVal(), true);
                    response = "Error saving CLIENT";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CLIENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating Client";
                Audit.InsertAudit((int)eventzz.ERROR_CLIENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Project(string Project_title, string Organization)
        { 
            string response = ""; 
            Project c = new Project();  
            string data = "";
            try
            { 

                CAEMS_Project cust = new CAEMS_Project(); 
                cust.Project_title =  Project_title;
                data += ",Project_title : " + Project_title;
                cust.Organization = long.Parse(Organization == null ? "1" : Organization);
                data += ",Organization : " + Organization; 
                response = c.add_Project(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_PROJECT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed Project add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_PROJECT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Project";
                Audit.InsertAudit((int)eventzz.ERROR_PROJECT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_Project> get_Project(string sql)
        { 
            List<CAEMS_Project> response = null;
            try
            { 
                Project c = new Project(); 
                response = c.get_Project(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_PROJECT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_Project2> get_Project2(string sql)
        { 
            List<CAEMS_Project2> response = null;
            try
            { 
                Project c = new Project();
                response = c.get_Project2(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_PROJECT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static List<CAEMS_Project3> get_Project3(string sql)
        { 
            List<CAEMS_Project3> response = null;
            try
            { 
                Project c = new Project();
                response = c.get_Project3( ); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_PROJECT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }


        public static string update_Project(string id, string oProject_title, string oOrganization,  string Project_title, string Organization,   bool andPassword = true) 
        { 
            string response = ""; 
            Project c = new Project();   
            string data = "";
            try
            { 
                CAEMS_Project cust = c.get_Project(" where id = " + id  )[0]; 
                cust.Project_title =  Project_title;
                data += ",Project_title : " + oProject_title + " -> " + Project_title;
                cust.Organization = long.Parse(Organization == null ? "1" : Organization);
                data += ",Organization : " + oOrganization + " -> " + Organization; 
                response = c.update_Project(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_PROJECT_UPDATE, data, getVal(), true);
                    response = "Error saving PROJECT";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_PROJECT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating Project";
                Audit.InsertAudit((int)eventzz.ERROR_PROJECT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_Admin(string Rightname)
        { 
            string response = ""; 
            right_Admin c = new right_Admin();  
            string data = "";
            try
            { 

                CAEMS_right_Admin cust = new CAEMS_right_Admin(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed right_Admin add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_right_Admin> get_right_Admin(string sql)
        { 
            List<CAEMS_right_Admin> response = null;
            try
            { 
                right_Admin c = new right_Admin(); 
                response = c.get_right_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_Admin( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_Admin c = new right_Admin();   
            string data = "";
            try
            { 
                CAEMS_right_Admin cust = c.get_right_Admin(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving RIGHT_ADMIN";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_User(string Rightname)
        { 
            string response = ""; 
            right_User c = new right_User();  
            string data = "";
            try
            { 

                CAEMS_right_User cust = new CAEMS_right_User(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_USER_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed right_User add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_USER_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_User";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_USER_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_right_User> get_right_User(string sql)
        { 
            List<CAEMS_right_User> response = null;
            try
            { 
                right_User c = new right_User(); 
                response = c.get_right_User(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_User( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_User c = new right_User();   
            string data = "";
            try
            { 
                CAEMS_right_User cust = c.get_right_User(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_USER_UPDATE, data, getVal(), true);
                    response = "Error saving RIGHT_USER";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_USER_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating right_User";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_USER_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_Admin(string Rolename, string selectedRights,string Service_company)
        { 
            string response = ""; 
            role_Admin c = new role_Admin();  
            string data = "";
            try
            { 

                CAEMS_role_Admin cust = new CAEMS_role_Admin(); 
                cust.Rolename =  Rolename;
                cust.service_company = long.Parse(Service_company);
                data += ",Rolename : " + Rolename;
                response = c.add_role_Admin(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed role_Admin add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_role_Admin> get_role_Admin(string sql)
        { 
            List<CAEMS_role_Admin> response = null;
            try
            { 
                role_Admin c = new role_Admin(); 
                response = c.get_role_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_Admin(string id, string oRolename, string Rolename, string selectedRights,   bool andPassword = true) 
        { 
            string response = ""; 
            role_Admin c = new role_Admin();   
            string data = "";
            try
            { 
                CAEMS_role_Admin cust = c.get_role_Admin(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename; 
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                response = c.update_role_Admin(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving ROLE_ADMIN";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating role_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_Admin(string Role,string Right)
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();  
            string data = "";
            try
            { 

                CAEMS_role_right_Admin cust = new CAEMS_role_right_Admin(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed role_right_Admin add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_role_right_Admin> get_role_right_Admin(string sql)
        { 
            List<CAEMS_role_right_Admin> response = null;
            try
            { 
                role_right_Admin c = new role_right_Admin(); 
                response = c.get_role_right_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_Admin( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();   
            string data = "";
            try
            { 
                CAEMS_role_right_Admin cust = c.get_role_right_Admin(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving ROLE_RIGHT_ADMIN";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating role_right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_User(string Role,string Right)
        { 
            string response = ""; 
            role_right_User c = new role_right_User();  
            string data = "";
            try
            { 

                CAEMS_role_right_User cust = new CAEMS_role_right_User(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_USER_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed role_right_User add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_USER_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_User";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_USER_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_role_right_User> get_role_right_User(string sql)
        { 
            List<CAEMS_role_right_User> response = null;
            try
            { 
                role_right_User c = new role_right_User(); 
                response = c.get_role_right_User(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_User( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_User c = new role_right_User();   
            string data = "";
            try
            { 
                CAEMS_role_right_User cust = c.get_role_right_User(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_User(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_USER_UPDATE, data, getVal(), true);
                    response = "Error saving ROLE_RIGHT_USER";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_USER_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating role_right_User";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_USER_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_User(string Rolename,string Client, string selectedRights, string Service_company)
        { 
            string response = ""; 
            role_User c = new role_User();  
            string data = "";
            try
            { 

                CAEMS_role_User cust = new CAEMS_role_User(); 
                cust.Rolename =  Rolename;
                cust.client = long.Parse(Client);
                data += ",Rolename : " + Rolename;
                response = c.add_role_User(cust, selectedRights); 
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_USER_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed role_User add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_USER_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_User";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_USER_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_role_User> get_role_User(string sql)
        { 
            List<CAEMS_role_User> response = null;
            try
            { 
                role_User c = new role_User(); 
                response = c.get_role_User(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_USER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_User(string id, string oRolename, string Rolename, string selectedRights, bool andPassword = true) 
        {

            //response = centralCalls.update_role_User(id: id, oRolename: oRolename, oClient: oClient, Rolename: Rolename, Client: Client, andPassword: false);

            string response = ""; 
            role_User c = new role_User();   
            string data = "";
            try
            { 
                CAEMS_role_User cust = c.get_role_User(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                response = c.update_role_User(cust, selectedRights);

                 
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_USER_UPDATE, data, getVal(), true);
                    response = "Error saving ROLE_USER";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_USER_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating role_User";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_USER_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Service_Company(string Company)
        { 
            string response = ""; 
            Service_Company c = new Service_Company();  
            string data = "";
            try
            { 

                CAEMS_Service_Company cust = new CAEMS_Service_Company(); 
                cust.Company =  Company;
                data += ",Company : " + Company;
                response = c.add_Service_Company(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_SERVICE_COMPANY_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed Service_Company add attempt";
                }
                else
                { 
                    response = "creation successful"; 
                    Audit.InsertAudit((int)eventzz.FAILED_SERVICE_COMPANY_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                throw d;
                response = "Error adding Service_Company";
                Audit.InsertAudit((int)eventzz.ERROR_SERVICE_COMPANY_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<CAEMS_Service_Company> get_Service_Company(string sql)
        { 
            List<CAEMS_Service_Company> response = null;
            try
            { 
                Service_Company c = new Service_Company(); 
                response = c.get_Service_Company(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_SERVICE_COMPANY_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        } 



        public static string update_Service_Company( string id, string oCompany,string Company, bool andPassword = true) 
        { 
            string response = ""; 
            Service_Company c = new Service_Company();   
            string data = "";
            try
            { 
                CAEMS_Service_Company cust = c.get_Service_Company(" where id = " + id  )[0]; 
                cust.Company =  Company;
                data += ",Company : " + oCompany + " -> " + Company;
                response = c.update_Service_Company(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_SERVICE_COMPANY_UPDATE, data, getVal(), true);
                    response = "Error saving SERVICE_COMPANY";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_SERVICE_COMPANY_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating Service_Company";
                Audit.InsertAudit((int)eventzz.ERROR_SERVICE_COMPANY_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
    }
}
