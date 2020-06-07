using System; 
using System.Net.Http; 
using System.Net; 
using Newtonsoft.Json; 
using System.Collections.Generic; 
using System.Linq; 
using System.Web; 
     
namespace sphinxsolaries.Caems 
{ 
    public class AuthorizeAttribute: System.Web.Http.AuthorizeAttribute 
    {    
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext) 
        {  
            if(!HttpContext.Current.User.Identity.IsAuthenticated) 
            {  
                base.HandleUnauthorizedRequest(actionContext);  
            } 
            else 
            {  
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden); 
            } 
        } 
    } 
} 
