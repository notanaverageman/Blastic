namespace Blastic.CodeGeneration.CSharp;

public class TryBuilder : BlockBuilder
{
	public TryBuilder(CodeBuilder codeBuilder) : base(codeBuilder)
	{
		CodeBuilder.AppendLine("try");
		CodeBuilder.AppendLine("{");
		CodeBuilder.Indent();
	}
}