namespace Findex.TechnicalTest.Domain.SesSnsEvents;

public class CommonHeaders
{
	public string? ReturnPath { get; set; }
	public List<string>? From { get; set; }
	public string? Date { get; set; }
	public List<string>? To { get; set; }
	public string? MessageId { get; set; }
	public string? Subject { get; set; }
}