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
    public class Project 
    { 
        public string add_Project(CAEMS_Project new_Project) 
         {
             string result = "";
             try
             {
              var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Insert<CAEMS_Project>(new_Project);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_Project(CAEMS_Project new_Project)
         {
             string result = "";
             try
             {
                 var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
                 var x = context.Update(new_Project);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<CAEMS_Project> get_Project(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             var actual = context.Fetch<CAEMS_Project>(sql);
             return actual;
         }  
         public List<CAEMS_Project2> get_Project2(string sql)
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             //var actual = context.Fetch<CAEMS_Project2>(sql);
             //context.FetchMultiple
             var actual = context.FetchOneToMany<CAEMS_Project2>(x => x.Organization2,
                 "select a.* , b.* from CAEMS_Project a inner join (select * from CAEMS_Client " +  sql + ") b on a.Organization = b.id  ");
             

             return actual;
         }  
         public List<CAEMS_Project3> get_Project3( )
         {
             var context = sphinxsolaries.Caems.Data.Models.Caems.GetInstance();
             //var actual = context.Fetch<CAEMS_Project2>(sql);
             //context.FetchMultiple
             var actual = context.Fetch<CAEMS_Project3>("select p.id, p.Project_Title, c.Name Organization , s.Company Service_Company from  CAEMS_Project p  inner join CAEMS_Client c on c.id = p.Organization inner join CAEMS_Service_Company s on s.id = c.Service_Company order by  s.Company, c.Name, p.Project_Title");
             

             return actual;
         } 
     }



    public partial class CAEMS_Project3
    { 
        public long Id
        {
            get;
            set;
        } 
        public string Project_title
        {
            get;
            set;
        } 
        public string Organization
        {
            get ; 
            set ;  
        } 
        public string Service_Company
        {
            get ; 
            set ;  
        } 
    }



    public partial class CAEMS_Project2
    { 
        public long Id
        {
            get;
            set;
        } 
        public string Project_title
        {
            get;
            set;
        } 
        public long Organization
        {
            get ; 
            set ;  
        } 
        public List<CAEMS_Client> Organization2
        {
            get ; 
            set ;  
        } 
    }

 }
