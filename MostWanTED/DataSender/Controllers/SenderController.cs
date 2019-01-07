using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DataSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    { 
    // GET api/values
    [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "sender1", "sender2" };
        }

        // GET api/values
        [HttpGet("status")]
        public IActionResult Status() => Ok();

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<JObject> SendSurveyResults(JObject surveyResults)
        {
            return surveyResults;
        }
    }
}
