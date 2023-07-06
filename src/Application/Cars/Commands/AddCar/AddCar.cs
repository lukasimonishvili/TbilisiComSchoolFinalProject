using Microsoft.AspNetCore.Mvc;

namespace Application.Cars.Commands.AddCar
{
    public interface ICar
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


    [ApiController]
    [Route("/api/[Controller]")]
    public class AddCar : Controller
    {
        [HttpPost("add")]
        public IActionResult AddCarPost([FromBody] ICar car)
        {
            return Ok(car);
        }
    }
}
