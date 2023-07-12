using Findex.TechnicalTest.Helpers;
using MimeKit;
using System.Text;
using System.Text.RegularExpressions;

namespace Findex.TechnicalTest.Strategies;

public class ProcessJsonFromLinkInBody : IProcessJsonStrategy
{
	public async Task<Result> ProccessJson(MimeMessage mimeMessage)
	{
		var links = GetCleanUrlsFromEmailBody(mimeMessage);
		var link = links[0];
		using HttpClient httpClient = new HttpClient();
		var jsonBytes = await httpClient.GetByteArrayAsync(link);
		string json = Encoding.UTF8.GetString(jsonBytes);
		return Result<string>.Ok(json);
	}

	// Método para obtener las URLs dentro del cuerpo del correo y limpiarlas
	private List<string> GetCleanUrlsFromEmailBody(MimeMessage message)
	{
		List<string> urls = new List<string>();

		foreach (MimeEntity entity in message.BodyParts)
		{
			if (entity is TextPart textPart)
			{
				string bodyText = textPart.Text;

				// Utiliza una expresión regular para encontrar todas las URLs en el cuerpo del correo
				Regex urlRegex = new Regex(@"(https?://[^\s]+)");
				MatchCollection matches = urlRegex.Matches(bodyText);

				// Agrega las URLs encontradas a la lista después de limpiarlas
				foreach (Match match in matches.Cast<Match>())
				{
					string url = match.Value;
					url = CleanUrl(url); // Aplica la limpieza a la URL
					if (IsJsonUrl(url)) // Verifica si la URL apunta a un archivo JSON
					{
						urls.Add(url);
					}
				}
			}
		}

		return urls;
	}

	// Método para limpiar una URL de caracteres no deseados
	private string CleanUrl(string url)
	{
		// Elimina caracteres no deseados como \\u003cspan
		url = Regex.Replace(url, @"\\[uU]([0-9A-Fa-f]{4})", "");

		return url;
	}

	// Método para verificar si una URL apunta a un archivo JSON
	private bool IsJsonUrl(string url)
	{
		// Verifica si la URL termina con .json
		return url.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
	}

}
