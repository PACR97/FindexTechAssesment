using Findex.TechnicalTest.Helpers;
using MimeKit;

namespace Findex.TechnicalTest.Strategies;

public class Context
{
	private IProcessJsonStrategy _strategy;

	public Context(IProcessJsonStrategy strategy)
	{
		_strategy = strategy;
	}

	public void SetStrategy(IProcessJsonStrategy strategy)
	{
		_strategy = strategy;
	}

	public async Task<Result> ExecuteStrategy(MimeMessage mimeMessage)
	{
		return await _strategy.ProccessJson(mimeMessage);
	}

}
