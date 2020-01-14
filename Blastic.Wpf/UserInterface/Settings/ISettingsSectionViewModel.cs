using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;

namespace Blastic.Wpf.UserInterface.Settings
{
	public interface ISettingsSectionViewModel : IHasLifetime
	{
		string SectionName { get; }
		IsExpandedSetting IsExpanded { get; }

		Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken);
	}
}