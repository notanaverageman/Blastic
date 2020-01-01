using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Blastic.Services.Dialog;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Windowing;

namespace Blastic.Execution
{
	public class ExecutionContextFactory
	{
		private readonly ILogger<ExecutionContext> _logger;
		private readonly IDialogService _dialogService;
		private readonly IWindowManager _windowManager;
		private readonly IEventAggregator _eventAggregator;
		private readonly INotificationService _notificationService;
		private readonly ISnackbarMessageQueue _messageQueue;

		public ExecutionContextFactory(
			ILogger<ExecutionContext> logger,
			IDialogService dialogService,
			IWindowManager windowManager,
			IEventAggregator eventAggregator,
			INotificationService notificationService,
			ISnackbarMessageQueue messageQueue)
		{
			_logger = logger;
			_dialogService = dialogService;
			_windowManager = windowManager;
			_eventAggregator = eventAggregator;
			_notificationService = notificationService;
			_messageQueue = messageQueue;
		}

		public ExecutionContext Create()
		{
			return new ExecutionContext(
				_logger,
				_dialogService,
				_windowManager,
				_eventAggregator,
				_notificationService,
				_messageQueue);
		}
	}
}