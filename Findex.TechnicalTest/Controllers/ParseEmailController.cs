using Findex.TechnicalTest.Helpers;
using Findex.TechnicalTest.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Findex.TechnicalTest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ParseEmailController : ControllerBase
	{
		[HttpGet("parseEmail")]
		public async Task<IActionResult> ParseEmail(string emailUrl)
		{
			try
			{
				byte[] emailBytes;
				if (IsLink(emailUrl))
				{
					// Descargar el archivo de correo
					using HttpClient httpClient = new();
					emailBytes = await httpClient.GetByteArrayAsync(emailUrl);
				}
				else
				{
					emailBytes = System.IO.File.ReadAllBytes(emailUrl);
				}
				

				// Crear una instancia de MimeMessage desde los bytes del correo
				MimeMessage message;
				using (MemoryStream memoryStream = new(emailBytes))
				{
					message = MimeMessage.Load(memoryStream);
				}

				bool containsJsonAttachment = FindJsonAttachment(message) != null;

				var context = new Context(new ProcessJsonFromAttachment());
				if (containsJsonAttachment)
				{
					var resultProcessJsonFromAttachment = await context.ExecuteStrategy(message);

					if (resultProcessJsonFromAttachment.Success)
					{
						var response = resultProcessJsonFromAttachment as Result<string>;
						// Devolver el JSON como respuesta
						return Content(response!.Value, "application/json", Encoding.UTF8);
					}
					if (resultProcessJsonFromAttachment.IsFailure)
					{
						return NotFound(resultProcessJsonFromAttachment.Error);
					}
				}
				else
				{
					bool containsLinkWithJsonInBody = message.TextBody.Contains(".json");
					if (containsLinkWithJsonInBody)
					{
						context.SetStrategy(new ProcessJsonFromLinkInBody());
						var resultProcessJsonFromBody = await context.ExecuteStrategy(message);
						if (resultProcessJsonFromBody.Success)
						{
							var response = resultProcessJsonFromBody as Result<string>;
							return Content(response!.Value, "application/json", Encoding.UTF8);
						}
						else
						{
							return NotFound(resultProcessJsonFromBody.Error);
						}
					}

				}


				return BadRequest("The request could not be processed");
			}
			catch (Exception ex)
			{
				// Manejar cualquier error ocurrido
				return BadRequest($"Ocurrió un error al procesar el correo: {ex.Message}");
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
		private string GetJsonContent(MimePart jsonAttachment)
		{
			using var memoryStream = new MemoryStream();
			jsonAttachment.Content.DecodeTo(memoryStream);
			memoryStream.Position = 0;

			using var streamReader = new StreamReader(memoryStream, Encoding.UTF8);
			return streamReader.ReadToEnd();
		}

		static bool IsLink(string input)
		{
			// Verificar si el string cumple con el patrón de una URL
			return Regex.IsMatch(input, @"^(http|https)://");
		}

		static bool IsLocalPath(string input)
		{
			// Verificar si el string es una ruta local en formato "C:\"
			return Path.IsPathRooted(input) && !IsLink(input);
		}

		private List<string> GetLinksFromBody(string body)
		{
			var links = new List<string>();
			// Expresión regular para encontrar enlaces que finalizan en .json
			var regex = new Regex(@"\b(?:https?://|www\.)\S+\.json\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

			// Buscar los enlaces en el cuerpo del mensaje
			var matches = regex.Matches(body);
			foreach (Match match in matches.Cast<Match>())
			{
				links.Add(match.Value);
			}

			return links;
		}
	}
}
