using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.Reactive;

namespace Blastic.Execution
{
	public class ExecutionContext
	{
		public IReactiveProperty<bool> IsBusy { get; set; }
		public IReactiveProperty<string> ProgressMessage { get; set; }
		public ReactiveCollection<string> ProgressDetails { get; }

		public IReactiveProperty<bool> IsCancellationSupported { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public IReactiveProperty<bool> IsShowingForm { get; set; }
		public IReactiveProperty<DynamicModel> Form { get; set; }

		public ExecutionContext()
		{
			IsBusy = new ReactiveProperty<bool>();
			ProgressMessage = new ReactiveProperty<string>();
			ProgressDetails = new ReactiveCollection<string>();

			IsCancellationSupported = new ReactiveProperty<bool>();
			CancellationTokenSource = new CancellationTokenSource();

			IsShowingForm = new ReactiveProperty<bool>();
			Form = new ReactiveProperty<DynamicModel>();
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

				ProgressDetails.Clear();
				IsCancellationSupported.Value = isCancellationSupported;
				
				if (CancellationTokenSource.IsCancellationRequested)
				{
					CancellationTokenSource?.Dispose();
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