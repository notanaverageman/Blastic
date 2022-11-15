using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.Execution
{
	/// <summary>
	/// A class that is mostly used on view models to enable cancellation, progress, and
	/// busyness.
	/// </summary>
	public class ExecutionContext
	{
		private readonly SourceList<string> _progressDetails;
		
		public IReactiveProperty<bool> IsBusy { get; }
		public IReactiveProperty<string> ProgressMessage { get; }
		public ReadOnlyObservableCollection<string> ProgressDetails { get; }

		public IReactiveProperty<bool> IsCancellationSupported { get; }
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public IReactiveProperty<bool> IsShowingForm { get; }
		public IReactiveProperty<DynamicModel?> Form { get; }

		public ExecutionContext()
		{
			_progressDetails = new SourceList<string>();

			_progressDetails
				.Connect()
				.ObserveOnUI()
				.Bind(out ReadOnlyObservableCollection<string> progressDetails)
				.Subscribe();

			ProgressDetails = progressDetails;
			
			IsBusy = new ReactiveProperty<bool>(false);
			ProgressMessage = new ReactiveProperty<string>("");

			IsCancellationSupported = new ReactiveProperty<bool>(false);
			CancellationTokenSource = new CancellationTokenSource();

			IsShowingForm = new ReactiveProperty<bool>(false);
			Form = new ReactiveProperty<DynamicModel?>(default);
		}

		public async Task Execute(
			Func<CancellationToken, Task> function,
			string progressMessage = "",
			bool showProgress = true,
			bool rethrowUnhandledException = false,
			bool isCancellationSupported = true,
			CancellationToken? customCancellationToken = null)
		{
			try
			{
				if (showProgress)
				{
					IsBusy.Value = true;
					ProgressMessage.Value = progressMessage;
				}

				_progressDetails.Clear();
				IsCancellationSupported.Value = isCancellationSupported;
				
				if (CancellationTokenSource.IsCancellationRequested)
				{
					CancellationTokenSource.Dispose();
					CancellationTokenSource = new CancellationTokenSource();
				}

				CancellationToken cancellationToken = customCancellationToken ?? CancellationTokenSource.Token;

				await function(cancellationToken);
			}
			catch (TaskCanceledException)
			{
			}
			catch
			{
				if (rethrowUnhandledException)
				{
					throw;
				}
			}
			finally
			{
				if (showProgress)
				{
					IsBusy.Value = false;
				}
			}
		}

		public void AddProgressDetail(string progressDetail)
		{
			_progressDetails.Add(progressDetail);
		}

		public async Task<bool> ShowForm(DynamicModel form)
		{
			if (form == null)
			{
				throw new ArgumentNullException(nameof(form));
			}

			try
			{
				Form.Value = form;
				IsShowingForm.Value = true;

				return await Form.Value.WaitCompletion();
			}
			finally
			{
				IsShowingForm.Value = false;
			}
		}
	}
}