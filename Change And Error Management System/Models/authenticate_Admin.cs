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
    public class authenticate_Admin 
    { 
        public string add_authenticate_Admin(CAEMS_authenticate_Admin new_authenticate_Admin) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_authenticate_Admin>(new_authenticate_Admin);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_authenticate_Admin(CAEMS_authenticate_Admin new_authenticate_Admin)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_authenticate_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_authenticate_Admin> get_authenticate_Admin(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             //context.
             var actual = context.Fetch<CAEMS_authenticate_Admin>(sql);
             return actual;
         }
         public List<CAEMS_authenticate_Admin2> get_authenticate_Admin2(string service_company_id)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             //context.
             var actual = context.FetchOneToMany<CAEMS_authenticate_Admin2>(x => x.Role2,
                 "select s.Company Service_Company2,a.* , b.* from CAEMS_authenticate_Admin a inner join CAEMS_role_Admin b on a.role = b.id  inner join CAEMS_Service_Company s on s.id = a.Service_Company" +
                 (service_company_id.Trim().Length > 0 ? " where  a.Service_Company = " + service_company_id + "  and  LeadAdmin =0 " : "") + 
                 
                 " order by "  +


                 (service_company_id.Trim().Length == 0 ? "   s.Company, " : "")  
                 
                 + "first_name, last_name, email");
             return actual;
         }

         public List<CAEMS_authenticate_Admin_And_User> get_Admin_And_User(string project)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance(); 
             var actual = context.Fetch<CAEMS_authenticate_Admin_And_User>(
                 "select  concat(first_name, ' ', last_name) name, email from CAEMS_authenticate_Admin where Service_Company in  ( select Service_Company from CAEMS_authenticate_User where  Organization in  " +
                 "(select Organization from CAEMS_Project where id = " + project + ")) union all " +
                 "select concat(first_name, ' ', last_name) name, email from CAEMS_authenticate_User where  Organization in " +
                 "(select Organization from CAEMS_Project where id = " + project + ") "
                 );
             return actual;
         }
     }




    public partial class CAEMS_authenticate_Admin2  
    {
        public long Id
        {
            get;
            set;
        }

        public string First_name
        {
            get;
            set;
        }
        public string Last_name
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        } 
        public long Role
        {
            get;
            set;
        }
        public List<CAEMS_role_Admin> Role2
        {
            get;
            set;
        }

        public byte[] Password
        {
            get;
            set;
        }
        public byte[] Password2
        {
            get;
            set;
        }
        public long Service_company
        {
            get;
            set;
        }
        public string Service_company2
        {
            get;
            set;
        }

        public Int16 Leadadmin
        {
            get;
            set;
        } 
    }





    public partial class CAEMS_authenticate_Admin_And_User  
    { 

        public string name
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }  
    }


 }
