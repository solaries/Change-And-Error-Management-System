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
    public class authenticate_SuperAdmin 
    { 
        public string add_authenticate_SuperAdmin(CAEMS_authenticate_SuperAdmin new_authenticate_SuperAdmin) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_authenticate_SuperAdmin>(new_authenticate_SuperAdmin);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_authenticate_SuperAdmin(CAEMS_authenticate_SuperAdmin new_authenticate_SuperAdmin)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_authenticate_SuperAdmin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_authenticate_SuperAdmin> get_authenticate_SuperAdmin(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_authenticate_SuperAdmin>(sql);
             return actual;
         }  
     }
 }
