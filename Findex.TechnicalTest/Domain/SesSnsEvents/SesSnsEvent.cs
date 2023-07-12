using Findex.TechnicalTest.Helpers;
using System.ComponentModel;

namespace Findex.TechnicalTest.Domain.SesSnsEvents;

[TypeConverter(typeof(SesSnsEventConverter))]
public class SesSnsEvent
{
	public string? EventVersion { get; set; }
	public Ses? Ses { get; set; }
	public string? EventSource { get; set; }
}
