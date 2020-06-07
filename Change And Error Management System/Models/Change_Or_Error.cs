using NPoco;
using sphinxsolaries.Caems.Data;
using sphinxsolaries.Caems.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
 
namespace sphinxsolaries.Caems.Models 
{ 
    public class Change_Or_Error 
    { 
        public string add_Change_Or_Error(CAEMS_Change_Or_Error new_Change_Or_Error) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_Change_Or_Error>(new_Change_Or_Error);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_Change_Or_Error(CAEMS_Change_Or_Error new_Change_Or_Error)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_Change_Or_Error);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_Change_Or_Error> get_Change_Or_Error(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Change_Or_Error>(sql);
             return actual;
         }  
         public List<CAEMS_Change_Or_Error2> get_Change_Or_Error2(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Change_Or_Error2>(" select * from (select a.id, a.Project, p.Project_Title Project2 ,a.Change_Or_Error_Detail,a.Initiating_User_Type,  Initiating_User_Type2, User2,   a.User,a.Log_Date from CAEMS_Change_Or_Error a   inner join CAEMS_Project p on a.Project = p.id inner join (select  'Client' Initiating_User_Type2, 2 UserType, a.id, concat(b.Name, ': ', a.first_name , ' ' , a.last_name) User2  from CAEMS_authenticate_User a inner join CAEMS_Client b on a.Organization = b.id union all   select   'Administrator' Initiating_User_Type2, 1 UserType, a.id, concat(b.Company, ': ', a.first_name , ' ' , a.last_name) User2 from CAEMS_authenticate_Admin a  inner join CAEMS_Service_Company b on a.Service_Company = b.id) b on a.User =b.id and  a.Initiating_User_Type = b.UserType) a    "
                 + sql + "order by Log_date desc");
             return actual;
         } 
     }



    public partial class CAEMS_Change_Or_Error2
    {
        public long Id
        {
            get;
            set;
        }
        public long Project
        {
            get;
            set;
        }
        public string Project2
        {
            get;
            set;
        }
        public string Initiating_User_Type2
        {
            get;
            set;
        }
        public string User2
        {
            get;
            set;
        }
        public long Initiating_User_Type
        {
            get;
            set;
        }
        public long User
        {
            get;
            set;
        }
        public string Change_or_error_detail
        {
            get;
            set;
        }
        public DateTime Log_date
        {
            get;
            set;
        }
 
    }

 }
