namespace Blastic.CodeGeneration.CSharp;

public class StructBuilder : TypeBuilder
{
	public StructBuilder(CodeBuilder codeBuilder, string name) : base(codeBuilder, "struct", name)
	{
	}
}