using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography; 
using System.Text;
using sphinxsolaries.Caems.Data.Models;

namespace sphinxsolaries.Caems.BusinessLogic
{ 
        public enum eventzz 
        { 
        SUCCESSFUL_AUTHENTICATE_ADMIN_ADD = 1,
FAILED_AUTHENTICATE_ADMIN_ADD = 2,
ERROR_AUTHENTICATE_ADMIN_ADD = 3,
SUCCESSFUL_AUTHENTICATE_ADMIN_GET = 4,
FAILED_AUTHENTICATE_ADMIN_GET = 5,
ERROR_AUTHENTICATE_ADMIN_GET = 6,
SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE = 7,
FAILED_AUTHENTICATE_ADMIN_UPDATE = 8,
ERROR_AUTHENTICATE_ADMIN_UPDATE = 9,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_ADD = 10,
FAILED_AUTHENTICATE_SUPERADMIN_ADD = 11,
ERROR_AUTHENTICATE_SUPERADMIN_ADD = 12,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_GET = 13,
FAILED_AUTHENTICATE_SUPERADMIN_GET = 14,
ERROR_AUTHENTICATE_SUPERADMIN_GET = 15,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_UPDATE = 16,
FAILED_AUTHENTICATE_SUPERADMIN_UPDATE = 17,
ERROR_AUTHENTICATE_SUPERADMIN_UPDATE = 18,
SUCCESSFUL_AUTHENTICATE_USER_ADD = 19,
FAILED_AUTHENTICATE_USER_ADD = 20,
ERROR_AUTHENTICATE_USER_ADD = 21,
SUCCESSFUL_AUTHENTICATE_USER_GET = 22,
FAILED_AUTHENTICATE_USER_GET = 23,
ERROR_AUTHENTICATE_USER_GET = 24,
SUCCESSFUL_AUTHENTICATE_USER_UPDATE = 25,
FAILED_AUTHENTICATE_USER_UPDATE = 26,
ERROR_AUTHENTICATE_USER_UPDATE = 27,
SUCCESSFUL_CHANGE_OR_ERROR_ADD = 28,
FAILED_CHANGE_OR_ERROR_ADD = 29,
ERROR_CHANGE_OR_ERROR_ADD = 30,
SUCCESSFUL_CHANGE_OR_ERROR_GET = 31,
FAILED_CHANGE_OR_ERROR_GET = 32,
ERROR_CHANGE_OR_ERROR_GET = 33,
SUCCESSFUL_CHANGE_OR_ERROR_UPDATE = 34,
FAILED_CHANGE_OR_ERROR_UPDATE = 35,
ERROR_CHANGE_OR_ERROR_UPDATE = 36,
SUCCESSFUL_CHANGE_OR_ERROR_MOVEMENT_ADD = 37,
FAILED_CHANGE_OR_ERROR_MOVEMENT_ADD = 38,
ERROR_CHANGE_OR_ERROR_MOVEMENT_ADD = 39,
SUCCESSFUL_CHANGE_OR_ERROR_MOVEMENT_GET = 40,
FAILED_CHANGE_OR_ERROR_MOVEMENT_GET = 41,
ERROR_CHANGE_OR_ERROR_MOVEMENT_GET = 42,
SUCCESSFUL_CHANGE_OR_ERROR_MOVEMENT_UPDATE = 43,
FAILED_CHANGE_OR_ERROR_MOVEMENT_UPDATE = 44,
ERROR_CHANGE_OR_ERROR_MOVEMENT_UPDATE = 45,
SUCCESSFUL_CLIENT_ADD = 46,
FAILED_CLIENT_ADD = 47,
ERROR_CLIENT_ADD = 48,
SUCCESSFUL_CLIENT_GET = 49,
FAILED_CLIENT_GET = 50,
ERROR_CLIENT_GET = 51,
SUCCESSFUL_CLIENT_UPDATE = 52,
FAILED_CLIENT_UPDATE = 53,
ERROR_CLIENT_UPDATE = 54,
SUCCESSFUL_PROJECT_ADD = 55,
FAILED_PROJECT_ADD = 56,
ERROR_PROJECT_ADD = 57,
SUCCESSFUL_PROJECT_GET = 58,
FAILED_PROJECT_GET = 59,
ERROR_PROJECT_GET = 60,
SUCCESSFUL_PROJECT_UPDATE = 61,
FAILED_PROJECT_UPDATE = 62,
ERROR_PROJECT_UPDATE = 63,
SUCCESSFUL_RIGHT_ADMIN_ADD = 64,
FAILED_RIGHT_ADMIN_ADD = 65,
ERROR_RIGHT_ADMIN_ADD = 66,
SUCCESSFUL_RIGHT_ADMIN_GET = 67,
FAILED_RIGHT_ADMIN_GET = 68,
ERROR_RIGHT_ADMIN_GET = 69,
SUCCESSFUL_RIGHT_ADMIN_UPDATE = 70,
FAILED_RIGHT_ADMIN_UPDATE = 71,
ERROR_RIGHT_ADMIN_UPDATE = 72,
SUCCESSFUL_RIGHT_USER_ADD = 73,
FAILED_RIGHT_USER_ADD = 74,
ERROR_RIGHT_USER_ADD = 75,
SUCCESSFUL_RIGHT_USER_GET = 76,
FAILED_RIGHT_USER_GET = 77,
ERROR_RIGHT_USER_GET = 78,
SUCCESSFUL_RIGHT_USER_UPDATE = 79,
FAILED_RIGHT_USER_UPDATE = 80,
ERROR_RIGHT_USER_UPDATE = 81,
SUCCESSFUL_ROLE_ADMIN_ADD = 82,
FAILED_ROLE_ADMIN_ADD = 83,
ERROR_ROLE_ADMIN_ADD = 84,
SUCCESSFUL_ROLE_ADMIN_GET = 85,
FAILED_ROLE_ADMIN_GET = 86,
ERROR_ROLE_ADMIN_GET = 87,
SUCCESSFUL_ROLE_ADMIN_UPDATE = 88,
FAILED_ROLE_ADMIN_UPDATE = 89,
ERROR_ROLE_ADMIN_UPDATE = 90,
SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD = 91,
FAILED_ROLE_RIGHT_ADMIN_ADD = 92,
ERROR_ROLE_RIGHT_ADMIN_ADD = 93,
SUCCESSFUL_ROLE_RIGHT_ADMIN_GET = 94,
FAILED_ROLE_RIGHT_ADMIN_GET = 95,
ERROR_ROLE_RIGHT_ADMIN_GET = 96,
SUCCESSFUL_ROLE_RIGHT_ADMIN_UPDATE = 97,
FAILED_ROLE_RIGHT_ADMIN_UPDATE = 98,
ERROR_ROLE_RIGHT_ADMIN_UPDATE = 99,
SUCCESSFUL_ROLE_RIGHT_USER_ADD = 100,
FAILED_ROLE_RIGHT_USER_ADD = 101,
ERROR_ROLE_RIGHT_USER_ADD = 102,
SUCCESSFUL_ROLE_RIGHT_USER_GET = 103,
FAILED_ROLE_RIGHT_USER_GET = 104,
ERROR_ROLE_RIGHT_USER_GET = 105,
SUCCESSFUL_ROLE_RIGHT_USER_UPDATE = 106,
FAILED_ROLE_RIGHT_USER_UPDATE = 107,
ERROR_ROLE_RIGHT_USER_UPDATE = 108,
SUCCESSFUL_ROLE_USER_ADD = 109,
FAILED_ROLE_USER_ADD = 110,
ERROR_ROLE_USER_ADD = 111,
SUCCESSFUL_ROLE_USER_GET = 112,
FAILED_ROLE_USER_GET = 113,
ERROR_ROLE_USER_GET = 114,
SUCCESSFUL_ROLE_USER_UPDATE = 115,
FAILED_ROLE_USER_UPDATE = 116,
ERROR_ROLE_USER_UPDATE = 117,
SUCCESSFUL_SERVICE_COMPANY_ADD = 118,
FAILED_SERVICE_COMPANY_ADD = 119,
ERROR_SERVICE_COMPANY_ADD = 120,
SUCCESSFUL_SERVICE_COMPANY_GET = 121,
FAILED_SERVICE_COMPANY_GET = 122,
ERROR_SERVICE_COMPANY_GET = 123,
SUCCESSFUL_SERVICE_COMPANY_UPDATE = 124,
FAILED_SERVICE_COMPANY_UPDATE = 125,
ERROR_SERVICE_COMPANY_UPDATE = 126 
        } 
    public class Audit 
    {  
        public static void InsertAudit(EventLog newEvent, string callerFormName)
        {
            var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
            try
            {
                context.Insert<EventLog>(newEvent);
            }
            catch (Exception ex)
            {
            
            }
        }
        public static void InsertAudit(int eventId, string eventDetails, long userId, bool userevent)
        { 
            EventLog audit = new EventLog();
            audit.Description =  eventDetails;
            audit.Eventid = eventId;
            audit.Userid = userId;
            audit.Userevent = userevent;
            audit.Eventdate = DateTime.Now;
            InsertAudit(audit, "");
        }
        public static string GetEncodedHash(string password, string salt)
        {
           MD5 md5 = new MD5CryptoServiceProvider();
           byte [] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
           string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
           return base64digest.Substring(0, base64digest.Length-2);
        }

        public static string SendMail(string email, string mailSubject, string mailBody, string callerFormName)
        {
            bool iSBodyHtml = false;
            string result = "";
            try
            {
                  MailMessage mail = new MailMessage();
                  mail.From = new MailAddress(ConfigurationManager.AppSettings["email"]);
                  mail.To.Add(email);
                  mail.Subject = mailSubject;
                  mail.IsBodyHtml = true;
                  mail.Body = mailBody;
                  SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["client"]);
                  smtp.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                  smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username_email"], ConfigurationManager.AppSettings["password"]); 
                  smtp.Send(mail); 
            }
            catch (Exception ex)
            { 
                result = ex.Message;
            }
            finally
            { 
            }
            return result;
        }

        public static string GenerateRandom()
        {
            string result = "";
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            result = new String(stringChars);
            return result;
        }    }
}
