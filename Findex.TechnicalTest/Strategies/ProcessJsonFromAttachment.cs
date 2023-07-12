using Findex.TechnicalTest.Helpers;
using MimeKit;
using System.Text;

namespace Findex.TechnicalTest.Strategies;

public class ProcessJsonFromAttachment : IProcessJsonStrategy
{
	public async Task<Result> ProccessJson(MimeMessage mimeMessage)
	{
		// Buscar el adjunto JSON en el correo
		MimePart? jsonAttachment = FindJsonAttachment(mimeMessage);
		if(jsonAttachment != null)
		{
			string json = GetJsonContent(jsonAttachment);
			return await Task.FromResult(Result.Ok<string>(json));
		}
		else
		{
			return Result.Fail<string>("Json attachment could not be found in the email", ResultErrorType.NotFound);
		}
	}

	// Método para buscar el adjunto JSON en el correo
	private static MimePart? FindJsonAttachment(MimeMessage message)
	{

		foreach (var attachment in message.Attachments)
		{
			if (attachment is MimePart jsonAttachment && jsonAttachment.ContentType.MimeType == "application/json")
			{
				return jsonAttachment;
			}
		}

		return null;
	}

	// Método para obtener el contenido del adjunto JSON
	private static string GetJsonContent(MimePart jsonAttachment)
	{
		using var memoryStream = new MemoryStream();
		jsonAttachment.Content.DecodeTo(memoryStream);
		memoryStream.Position = 0;

		using var streamReader = new StreamReader(memoryStream, Encoding.UTF8);
		return streamReader.ReadToEnd();
	}
}
