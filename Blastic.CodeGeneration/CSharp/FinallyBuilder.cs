namespace Blastic.CodeGeneration.CSharp;

public class FinallyBuilder : BlockBuilder
{
	public FinallyBuilder(CodeBuilder codeBuilder) : base(codeBuilder)
	{
		CodeBuilder.AppendLine("finally");
		CodeBuilder.AppendLine("{");
		CodeBuilder.Indent();
	}
}