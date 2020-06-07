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
    public class role_right_Admin 
    { 
        public string add_role_right_Admin(CAEMS_role_right_Admin new_role_right_Admin) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_role_right_Admin>(new_role_right_Admin);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_role_right_Admin(CAEMS_role_right_Admin new_role_right_Admin)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_role_right_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_role_right_Admin> get_role_right_Admin(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_role_right_Admin>(sql);
             return actual;
         }  
     }
 }
