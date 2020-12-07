using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Blastic.CodeGeneration.Test
{
	public class ResxAdditionalText : AdditionalText
	{
		private readonly string _text;

		public override string Path { get; }

		public ResxAdditionalText(string resxPath)
		{
			Path = resxPath;
			_text = File.ReadAllText(resxPath);
		}

		public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken())
		{
			return SourceText.From(_text);
		}
	}
}