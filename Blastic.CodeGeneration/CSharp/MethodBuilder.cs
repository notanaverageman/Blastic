namespace Blastic.CodeGeneration.CSharp;

public class MethodBuilder : MethodBaseBuilder
{
	public MethodBuilder(
		CodeBuilder codeBuilder,
		string returnType,
		string name)
		:
		base(codeBuilder, $"{returnType} {name}", "public")
	{
	}
}