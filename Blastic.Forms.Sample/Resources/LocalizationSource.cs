using Blastic.CodeGeneration;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.Resources;

[ResxLocalizationSource("Resources.resx")]
[ResxLocalizationSource("Resources.tr-tr.resx")]
public partial class LocalizationSource
{
	public LocalizationSource(Order order)
	{
		Order = order;
	}
}