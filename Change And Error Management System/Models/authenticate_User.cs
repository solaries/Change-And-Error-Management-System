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
    public class authenticate_User 
    { 
        public string add_authenticate_User(CAEMS_authenticate_User new_authenticate_User) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_authenticate_User>(new_authenticate_User);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_authenticate_User(CAEMS_authenticate_User new_authenticate_User)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_authenticate_User);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_authenticate_User> get_authenticate_User(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_authenticate_User>(sql);
             
             return actual;
         }

         public List<CAEMS_authenticate_User3> get_authenticate_User3(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_authenticate_User3>("select a.`id` , `first_name` , `last_name` , `email` , b.roleName `role2` ,  `role` , `password` , `password2` , `Organization`  , `Lead_User`  , `Service_Company`  from CAEMS_authenticate_User a inner join CAEMS_role_User b on a.role = b.id " + sql);
             
             return actual;
         }
         public List<CAEMS_authenticate_User2> get_authenticate_User2(string service_company_id)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.FetchOneToMany<CAEMS_authenticate_User2>(x => x.Organization2,
                 "select  s.Company Service_Company2,r.roleName,a.* , b.* from CAEMS_authenticate_User a inner join CAEMS_Client b on a.Organization = b.id  inner join CAEMS_Service_Company s on s.id = a.Service_Company"
                 + "  inner join CAEMS_role_User r on r.id = a.role "
                 + (service_company_id.Trim().Length > 0 ? " where  a.Service_Company = " + service_company_id + "  and   Lead_User =1 " : "") +  
                 " order by " +
                (service_company_id.Trim().Length == 0 ? "   s.Company, b.Name, Lead_User,r.roleName, " : "")

                 + " first_name, last_name, email"); 
             
             return actual;
         }  
     }



    public partial class CAEMS_authenticate_User2
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

        public long Organization
        {
            get;
            set;
        }

        public List<CAEMS_Client> Organization2
        {
            get;
            set;
        }

        public Int16 Lead_user
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
        public string roleName
        {
            get;
            set;
        }

    }


    public partial class CAEMS_authenticate_User3
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

        public string Role2
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

        public long Organization
        {
            get;
            set;
        }
         

        public Int16 Lead_user
        {
            get;
            set;
        }

        public long Service_company
        {
            get;
            set;
        }
        public string  Service_company2
        {
            get;
            set;
        }

    }

 }
