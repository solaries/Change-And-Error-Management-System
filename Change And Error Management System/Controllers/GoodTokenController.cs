using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Net; 
using System.Net.Http; 
using System.Web.Http; 
     
namespace  sphinxsolaries.Caems.Controllers 
{ 
    public class GoodTokenController : ApiController 
    {  
        [Authorize] 
        public string Get() 
        { 
            return "value"; 
        }  
    } 
} 
