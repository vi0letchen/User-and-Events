using Microsoft.AspNetCore.Mvc;
using A2.Models;
using A2.Data;
using Microsoft.AspNetCore.Authorization;
using A2.Dtos;
using A2.Helper;

namespace A2.Controllers
{
    [Route("webapi")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repository;

        public A2Controller(IA2Repo repository)
        {
            _repository = repository;
        }

        [HttpPost("Register")]
        public ActionResult Register(User newUser)
        {
            var existingUser = _repository.GetUserByName(newUser.UserName); ;
            if (existingUser != null)
            {

                return Ok($"UserName {newUser.UserName} is not available.");

            }
            _repository.AddUser(newUser);

            return Ok("User successfully registered.");
        }


        [Authorize(AuthenticationSchemes = "Basic")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PurchaseSign/{signId}")]
        public ActionResult PurchaseSign(string signId)
        {

            string username = User.Identity.Name;

            var sign = _repository.GetSignById(signId);
            if (sign == null)
            {
                return BadRequest($"Sign {signId} not found");
            }

            var purchaseOutput = new PurchaseOutput
            {
                UserName = username,
                SignID = signId,
            };

            return Ok(purchaseOutput);

        }

        [Authorize(AuthenticationSchemes = "Basic")]
        [Authorize(Policy = "OrganizerOnly")]
        [HttpPost("AddEvent")]
        public ActionResult AddEvent(EventInput eventInput)
        {
            string username = User.Identity.Name;


            bool isEndValid = DateTime.TryParseExact(
                eventInput.End,
                "yyyyMMddTHHmmssZ",
                null,
                System.Globalization.DateTimeStyles.AssumeUniversal,
                out DateTime endDate);
            bool isStartValid = DateTime.TryParseExact(
                eventInput.Start,
                "yyyyMMddTHHmmssZ",
                null,
                System.Globalization.DateTimeStyles.AssumeUniversal,
                out DateTime startDate);

            if (!isStartValid && !isEndValid)
            {
                return StatusCode(400, "The format of Start and End should be yyyyMMddTHHmmssZ.");
            }
            else if (!isStartValid)
            {
                return BadRequest("The format of Start should be yyyyMMddTHHmmssZ.");
            }
            else if (!isEndValid)
            {
                return BadRequest("The format of End should be yyyyMMddTHHmmssZ.");
            }

            var newEvent = new Event
            {
                Start = startDate.ToString("yyyyMMddTHHmmssZ"),
                End = endDate.ToString("yyyyMMddTHHmmssZ"),
                Summary = eventInput.Summary,
                Description = eventInput.Description,
                Location = eventInput.Location
            };

            _repository.AddEvent(newEvent);
            return Ok("Success");
        }

        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpGet("EventCount")]
        public ActionResult GetEvents()
        {
            string username = User.Identity.Name;
            var count = _repository.GetEventsCount();
            return Ok(count);

        }
        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpGet("Event/{id}")]
        public ActionResult GetEvent(int id)
        {
            string username = User.Identity.Name;
            Event event1 = _repository.GetEventById(id);
            if (event1 == null)
            {
                return BadRequest($"Event {id} does not exist.");
            }

            return new ObjectResult(event1)
            {
                Formatters = { new CalendarOutputFormatter() }
            };

        }


    }

}