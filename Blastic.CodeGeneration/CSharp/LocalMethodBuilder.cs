namespace Blastic.CodeGeneration.CSharp;

public class LocalMethodBuilder : MethodBaseBuilder
{
	public LocalMethodBuilder(
		CodeBuilder codeBuilder,
		string returnType,
		string name)
		:
		base(codeBuilder, $"{returnType} {name}", "")
	{
	}
}