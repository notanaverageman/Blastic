using System.Reactive.Linq;
using Blastic.Reactive;

namespace Blastic.Maui.Sample;

public class DiceHistogramEntry
{
	public int Number { get; }
	public IReactiveProperty<int> Count { get; }
	public IReactiveProperty<int> DiceCount { get; }
	public IReadOnlyReactiveProperty<float> Ratio { get; }

	public DiceHistogramEntry(int number)
	{
		Number = number;

		Count = new ReactiveProperty<int>(0);
		DiceCount = new ReactiveProperty<int>(0);

		Ratio = Count
			.CombineLatest(DiceCount)
			.Select(x => x.Second > 0 ? x.First / (float)x.Second : 0)
			.ToReadOnlyReactiveProperty(0);
	}
}