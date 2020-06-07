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
    public class Client 
    { 
        public string add_Client(CAEMS_Client new_Client) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_Client>(new_Client);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_Client(CAEMS_Client new_Client)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_Client);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_Client> get_Client(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Client>(sql);
             return actual;
         }  
         public List<CAEMS_Client2> get_Client2(string sql,string email)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             //var actual = context.Fetch<CAEMS_Client2>(sql);


             var actual = context.FetchOneToMany<CAEMS_Client2>(x => x.Service_company2,
                 "select a.* , b.* from CAEMS_Client a inner join (select * from CAEMS_Service_Company " + sql + ") b on a.Service_Company = b.id "
                 +  (email.Trim().Length > 0 ? " where a.id in  (select Organization from CAEMS_authenticate_User where  replace(email, '@','#') = '" + email.Replace("@", "#") + "' )  ": "") +
               
             
               "order by b.Company, a.Name");
             
             return actual;
         } 
     }



    public partial class CAEMS_Client2
    {
        
        public long Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public long Service_company
        {
            get;
            set;
        }
        public List<CAEMS_Service_Company> Service_company2
        {
            get;
            set;
        }
 
    }


 }
