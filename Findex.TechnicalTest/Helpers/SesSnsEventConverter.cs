using Findex.TechnicalTest.Domain.SesSnsEvents;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Findex.TechnicalTest.Features.SesSnsEvents.MapSesSnsEventsToRepsonse.MapSesSnsEventsToRepsonse;

namespace Findex.TechnicalTest.Helpers;

public class SesSnsEventConverter : TypeConverter
{
	public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
	{
		return destinationType == typeof(MapSesSnsEventsResponse);
	}

	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		ArgumentNullException.ThrowIfNull(value);
		var source = (SesSnsEvent)value;
		var result = new MapSesSnsEventsResponse()
		{
			Spam = IsStatusEqualsToPASS(source!.Ses!.Receipt!.SpamVerdict!.Status!),
			Virus = IsStatusEqualsToPASS(source!.Ses!.Receipt!.VirusVerdict!.Status!),
			Dns = IsValidDns(source!.Ses!.Receipt!.SpfVerdict!.Status!, source!.Ses!.Receipt!.DkimVerdict!.Status!, source.Ses!.Receipt!.DmarcVerdict!.Status!),
			Mes = source!.Ses!.Mail!.Timestamp!.ToString("MMMM").ToUpper(),
			Retrasado = source!.Ses!.Receipt!.ProcessingTimeMillis > 1000,
			Emisor = GetUserEmailWithoutDomain(source!.Ses!.Mail!.Source!),
			Receptor = GetUserReceptorsWithoutDomainFromEmail(source!.Ses!.Mail!.Destination!)
		};
		return result;
	}

	static bool IsStatusEqualsToPASS(string value)
	{
		return value == "PASS";
	}

	static bool IsValidDns(params string[] args)
	{
		return args.All(x => IsStatusEqualsToPASS(x));
	}

	static string GetUserEmailWithoutDomain(string email)
	{
		int indexOfAt = email.IndexOf("@");
		string userName = email[..indexOfAt];
		return userName;
	}

	static List<string> GetUserReceptorsWithoutDomainFromEmail(List<string> emails)
	{
		var receptors = emails.Select(x => GetUserEmailWithoutDomain(x)).ToList();

		return receptors;
	}
}
