using System.Text.Json.Serialization;

namespace Blastic.Maui.Sample;

public abstract class GameAction
{
	[JsonIgnore]
	public int Id { get; set; }

	[JsonIgnore]
	public virtual bool SupportsSuspendingRedraw => true;

	public abstract void Apply(IServiceProvider serviceProvider);
	public abstract void Undo(IServiceProvider serviceProvider);
}