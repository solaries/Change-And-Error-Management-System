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
    public class Change_Or_Error_Movement 
    { 
        public string add_Change_Or_Error_Movement(CAEMS_Change_Or_Error_Movement new_Change_Or_Error_Movement) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_Change_Or_Error_Movement>(new_Change_Or_Error_Movement);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_Change_Or_Error_Movement(CAEMS_Change_Or_Error_Movement new_Change_Or_Error_Movement)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_Change_Or_Error_Movement);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_Change_Or_Error_Movement> get_Change_Or_Error_Movement(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Change_Or_Error_Movement>(sql);
             return actual;
         }  
         public List<CAEMS_Change_Or_Error_Movement2> get_Change_Or_Error_Movement2(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Change_Or_Error_Movement2>("select id,Project, Project_Title, `Change`,Change2, `Action`, Action2 ,  Action_User_Type , Action_User_Type2 ,   Action_User, Action_User2,  Move_Date   from     (	select a.id,Project, Project_Title, a.`Change`,a.Change2,a.`Action`, a.Action2 ,  a.Action_User_Type , a.Action_User_Type2 ,    	a.Action_User, b.User2 Action_User2, a.Move_Date  	from  	( 	select 0 id,p.id Project, p.Project_Title,a.id `Change`,Change_Or_Error_Detail Change2, 1 `Action`, 'Submited'  Action2 ,   		Initiating_User_Type Action_User_Type , if (Initiating_User_Type=1, 'Administrator',if (Initiating_User_Type=2, 'Client','')) Action_User_Type2 ,    		`User` Action_User, Log_Date Move_Date   		from CAEMS_Change_Or_Error a   inner join CAEMS_Project p on Project = p.id 		union all    		select a.id,p.id Project, p.Project_Title, `Change`,Change_Or_Error_Detail , a.`Action`, if (a.`Action`=2, 'Being Processed',if(a.`Action`=3, 'Processed',if (a.`Action`=4, 'Processing Confirmed','')))  Action2 , 		a.Action_User_Type , if ( a.Action_User_Type=1, 'Administrator',if ( a.Action_User_Type=2, 'Client','')) Action_User_Type2 ,   		a.Action_User, a.Move_Date  		from  CAEMS_Change_Or_Error_Movement a  		inner join CAEMS_Change_Or_Error b on a.`Change` = b.id  inner join CAEMS_Project p on b.Project = p.id) a inner join ( select  'Client' Initiating_User_Type2, 2 UserType, a.id, concat(b.Name, ': ', a.first_name , ' ' , a.last_name) User2  from CAEMS_authenticate_User a inner join CAEMS_Client b on a.Organization = b.id  union all    select   'Administrator' Initiating_User_Type2, 1 UserType, a.id, concat(b.Company, ': ', a.first_name , ' ' , a.last_name) User2 from CAEMS_authenticate_Admin a  inner join CAEMS_Service_Company b on a.Service_Company = b.id ) b on a.Action_User_Type = b.UserType and a.Action_User = b.id) a   "
                 + "inner join  (select max(id) id1, `Change` c from ( select 0 id, id `Change`  from CAEMS_Change_Or_Error  union all select  max(id) id,`Change` from  CAEMS_Change_Or_Error_Movement group by `Change` ) a group by `Change`) b on  a.id = b.id1 and a.`Change` = b.c   "
                 + sql
                 + "   order by `Action` , Move_Date desc  "
                 );
             return actual;
         } 
     }




    public partial class CAEMS_Change_Or_Error_Movement2  
    { 
        public long Id
        {
            get;
            set;
        }
        public string Project_Title
        {
            get;
            set;
        }
        public long Change
        {
            get;
            set;
        }
        public string Change2
        {
            get;
            set;
        }
        public Int16 Action
        {
            get;
            set;
        }
        public string Action2
        {
            get;
            set;
        }
        public Int16 Action_user_type
        {
            get;
            set;
        }
        public string Action_user_type2
        {
            get;
            set;
        }
        public long Action_user
        {
            get;
            set;
        }
        public string Action_user2
        {
            get;
            set;
        }
        public DateTime Move_Date
        {
            get;
            set;
        } 
         
    }


 }
