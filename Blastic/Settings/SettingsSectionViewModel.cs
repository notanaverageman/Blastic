using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Settings;

public class SettingsSectionViewModel : ConductorAllActive<SettingGroup>
{
	public IReadOnlyReactiveProperty<string> SectionName { get; }

	public SettingsSectionViewModel(IReadOnlyReactiveProperty<string> sectionName)
	{
		SectionName = sectionName;
	}

	public async Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
	{
		IEnumerable<Task<IEnumerable<DiagnosticMessage>>> tasks = Items.Select(x => x.GetDiagnosticMessages(cancellationToken));
		IEnumerable<DiagnosticMessage>[] diagnosticMessagesCollection = await Task.WhenAll(tasks);

		return diagnosticMessagesCollection.SelectMany(x => x);
	}
}