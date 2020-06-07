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
    public class role_Admin 
    {
        public string add_role_Admin(CAEMS_role_Admin new_role_Admin, string selectedRights) 
         {
             string result = "";
             try
             {
                var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                var x = context.Insert<CAEMS_role_Admin>(new_role_Admin); 
                List<CAEMS_role_right_Admin> adminRoleRightList = new List<CAEMS_role_right_Admin>();
                string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < idList.Length; i++)
                {
                    CAEMS_role_right_Admin adminRoleRight = new CAEMS_role_right_Admin();
                    adminRoleRight.Role = long.Parse(x.ToString());
                    adminRoleRight.Right = long.Parse(idList[i]);
                    adminRoleRightList.Add(adminRoleRight);
                }
                context.InsertBulk<CAEMS_role_right_Admin>(adminRoleRightList); 
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
        public string update_role_Admin(CAEMS_role_Admin new_role_Admin, string selectedRights )
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_role_Admin);

                 //context.DeleteMany<CAEMS_role_right_Admin>;// (context.Fetch<CAEMS_role_Admin>(""));
                 context.DeleteWhere<CAEMS_role_right_Admin>(" role = " + new_role_Admin.Id.ToString());


                 List<CAEMS_role_right_Admin> adminRoleRightList = new List<CAEMS_role_right_Admin>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                     CAEMS_role_right_Admin adminRoleRight = new CAEMS_role_right_Admin();
                     adminRoleRight.Role = long.Parse(new_role_Admin.Id.ToString());
                     adminRoleRight.Right = long.Parse(idList[i]);
                     adminRoleRightList.Add(adminRoleRight);
                 }
                 context.InsertBulk<CAEMS_role_right_Admin>(adminRoleRightList);


             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_role_Admin> get_role_Admin(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_role_Admin>(sql);
             return actual;
         }  
     }
 }
