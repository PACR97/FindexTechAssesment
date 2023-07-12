using Findex.TechnicalTest.Domain.SesSnsEvents;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Findex.TechnicalTest.Features.SesSnsEvents.MapSesSnsEventsToRepsonse.MapSesSnsEventsToRepsonse;

namespace Findex.TechnicalTest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MapJsonController : ControllerBase
	{
		private readonly ISender _sender;

		public MapJsonController(ISender sender)
		{
			_sender = sender;
		}

		[HttpPost]
		public async Task<IActionResult> MapSesSnsEvent([FromBody] List<SesSnsEvent> records)
		{
			var query = new MapSesSnsEventsToRepsonseQuery(records);
			var result = await _sender.Send(query);
			return Ok(result);
		}
	}
}
