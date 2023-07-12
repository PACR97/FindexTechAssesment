using Findex.TechnicalTest.Helpers;
using MimeKit;

namespace Findex.TechnicalTest.Strategies;

public interface IProcessJsonStrategy
{
	Task<Result> ProccessJson(MimeMessage mimeMessage); 
}
