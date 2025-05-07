using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Moduls.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        IService _iService;

        public AppointmentsController(IService iService)
        {
            _iService = iService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _iService.GetAppointmentAsync(id);
         return Ok(result);   
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]NewAppointmentDto newAppointment)
        {
            try
            {
                await _iService.AddAppointmentsAsync(newAppointment);
            }
            catch (NotFoundExpection__)
            {
                return NotFound();
            }
            catch (ConflictExpection)
            {
                return Conflict();
            }
            
            return Created();
        }
    }
}
