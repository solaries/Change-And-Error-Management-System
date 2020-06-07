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
    public class role_User 
    {
        public string add_role_User(CAEMS_role_User new_role_User, string selectedRights) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
              var x = context.Insert<CAEMS_role_User>(new_role_User);

              List<CAEMS_role_right_User> userRoleRightList = new List<CAEMS_role_right_User>();
              string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
              for (int i = 0; i < idList.Length; i++)
              {
                  CAEMS_role_right_User userRoleRight = new CAEMS_role_right_User();
                  userRoleRight.Role = long.Parse(x.ToString());
                  userRoleRight.Right = long.Parse(idList[i]);
                  userRoleRightList.Add(userRoleRight);
              }
              context.InsertBulk<CAEMS_role_right_User>(userRoleRightList);

            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
        public string update_role_User(CAEMS_role_User new_role_User, string selectedRights)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_role_User);




                 context.DeleteWhere<CAEMS_role_right_User>(" role = " + new_role_User.Id.ToString());


                 List<CAEMS_role_right_User> userRoleRightList = new List<CAEMS_role_right_User>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                     CAEMS_role_right_User userRoleRight = new CAEMS_role_right_User();
                     userRoleRight.Role = long.Parse(new_role_User.Id.ToString());
                     userRoleRight.Right = long.Parse(idList[i]);
                     userRoleRightList.Add(userRoleRight);
                 }
                 context.InsertBulk<CAEMS_role_right_User>(userRoleRightList);

             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_role_User> get_role_User(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_role_User>(sql);
             return actual;
         }  
     }
 }
