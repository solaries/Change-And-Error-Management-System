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
    public class right_Admin 
    { 
        public string add_right_Admin(CAEMS_right_Admin new_right_Admin) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_right_Admin>(new_right_Admin);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_right_Admin(CAEMS_right_Admin new_right_Admin)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_right_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_right_Admin> get_right_Admin(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_right_Admin>(sql);
             return actual;
         }  
     }
 }
