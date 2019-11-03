using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Controls.DynamicControls;
using Blastic.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Blastic.Services.Dialog;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Events;
using Reactive.Bindings;

namespace Blastic.Execution
{
	public class ExecutionContext
	{
		public ILogger Logger { get; }
		public IDialogService DialogService { get; }
		public IWindowManager WindowManager { get; }
		public IEventAggregator EventAggregator { get; }
		public ISnackbarMessageQueue MessageQueue { get; }

		public IReactiveProperty<bool> IsBusy { get; set; }
		public IReactiveProperty<string> ProgressMessage { get; set; }
		public ReactiveCollection<string> ProgressDetails { get; }

		public IReactiveProperty<bool> IsCancellationSupported { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public IReactiveProperty<bool> IsShowingForm { get; set; }
		public IReactiveProperty<DynamicModel> Form { get; set; }

		public ExecutionContext(
			ILogger<ExecutionContext> logger,
			IDialogService dialogService,
			IWindowManager windowManager,
			IEventAggregator eventAggregator,
			ISnackbarMessageQueue messageQueue)
		{
			Logger = logger;
			DialogService = dialogService;
			WindowManager = windowManager;
			EventAggregator = eventAggregator;
			MessageQueue = messageQueue;

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
			string successMessage = "",
			string failMessage = "",
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

				if (!string.IsNullOrEmpty(successMessage))
				{
					MessageQueue.Enqueue(successMessage);
				}
			}
			catch (TaskCanceledException)
			{
			}
			catch (Exception exception)
			{
				string source = GetFunctionSignature(function);

				using (Logger.BeginScope(new Dictionary<string, object> { { "ExecutionContextSource", source } }))
				{
					Logger.LogError(exception, exception.Message);
				}

				MessageQueue.Enqueue(
					string.IsNullOrEmpty(failMessage)
						? exception.Message
						: failMessage,
					"Open Logs",
					() => EventAggregator.Publish(new OpenLogsEvent()));

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

		private string GetFunctionSignature(Func<CancellationToken, Task> function)
		{
			MethodInfo method = function.Method;
			Type type = method.DeclaringType;

			while (type?.DeclaringType != null && type.Name.StartsWith("<"))
			{
				type = type.DeclaringType;
			}

			string methodName = method.Name;

			int start = methodName.IndexOf("<");
			int end = methodName.IndexOf(">");

			if (start >= 0 && end >= 0)
			{
				methodName = methodName.Substring(start + 1, end - start - 1);
			}

			if (type != null)
			{
				return type.FullName + "." + methodName;
			}

			return typeof(ExecutionContext).FullName + "." + nameof(Execute);
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