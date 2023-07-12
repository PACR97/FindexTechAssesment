using Findex.TechnicalTest.Domain.SesSnsEvents;
using MediatR;
using Nelibur.ObjectMapper;

namespace Findex.TechnicalTest.Features.SesSnsEvents.MapSesSnsEventsToRepsonse;

public class MapSesSnsEventsToRepsonse
{
	public class MapSesSnsEventsToRepsonseQuery : IRequest<List<MapSesSnsEventsResponse>>
	{
		public MapSesSnsEventsToRepsonseQuery(List<SesSnsEvent> sesSnsEvents)
		{
			SesSnsEvents = sesSnsEvents;
		}
		public List<SesSnsEvent> SesSnsEvents { get; set; }

	}

	public class MapSesSnsEventsResponse
	{
		public bool Spam { get; set; }
		public bool Virus { get; set; }
		public bool Dns { get; set; }
		public string? Mes { get; set; }
		public bool Retrasado { get; set; }
		public string? Emisor { get; set; }
		public List<string> Receptor { get; set; } = new List<string>();
	}

	public class Handler : IRequestHandler<MapSesSnsEventsToRepsonseQuery, List<MapSesSnsEventsResponse>>
	{
		public Task<List<MapSesSnsEventsResponse>> Handle(MapSesSnsEventsToRepsonseQuery request, CancellationToken cancellationToken)
		{
			TinyMapper.Bind<List<SesSnsEvent>, List<MapSesSnsEventsResponse>>();

			var response = TinyMapper.Map<List<MapSesSnsEventsResponse>>(request.SesSnsEvents);
			return Task.FromResult(response);
		}
	}
}
