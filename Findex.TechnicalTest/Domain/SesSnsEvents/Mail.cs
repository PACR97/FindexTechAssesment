namespace Findex.TechnicalTest.Domain.SesSnsEvents;

public class Mail
{
	public DateTime Timestamp { get; set; }
	public string? Source { get; set; }
	public string? MessageId { get; set; }
	public List<string>? Destination { get; set; }
	public bool HeadersTruncated { get; set; }
	public List<Header>? Headers { get; set; }
	public CommonHeaders? CommonHeaders { get; set; }
}