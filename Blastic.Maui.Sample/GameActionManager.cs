using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.Maui.Sample.Data;
using Blastic.Reactive;
using DynamicData;
using DynamicData.Binding;
using Command = Blastic.Commanding.Command;

namespace Blastic.Maui.Sample;

public class GameActionManager
{
	public const int ActionCountToLoad = 10;
	public const int ActionCountToTriggerLoad = ActionCountToLoad / 2;

	private readonly IServiceProvider _serviceProvider;
	private readonly GameDatabase _database;

	private readonly ObservableCollectionExtended<GameAction> _undoActions;
	private readonly ObservableCollectionExtended<GameAction> _redoActions;

	private readonly IReactiveProperty<int?> _lastAppliedActionId;
	private readonly IReactiveProperty<int?> _lastAppliedActionIdSaved;

	private Game _game;

	// Actions are ordered from oldest to newest.
	// New actions are inserted to the end of the list.
	// The last action is applied when undo requested.
	public ReadOnlyObservableCollection<GameAction> UndoActions { get; }

	// Actions are ordered from newest to oldest.
	// New actions are inserted to the beginning of the list.
	// The last action is applied when redo requested.
	public ReadOnlyObservableCollection<GameAction> RedoActions { get; }

	public Command UndoCommand { get; }
	public Command RedoCommand { get; }

	public IReadOnlyReactiveProperty<bool> IsDirty { get; }

	public GameActionManager(
		IServiceProvider serviceProvider,
		GameDatabase database,
		Game game)
	{
		_serviceProvider = serviceProvider;
		_database = database;
		_game = game;

		_undoActions = new ObservableCollectionExtended<GameAction>();
		_redoActions = new ObservableCollectionExtended<GameAction>();

		_lastAppliedActionId = new ReactiveProperty<int?>(default);
		_lastAppliedActionIdSaved = new ReactiveProperty<int?>(default);

		UndoActions = new ReadOnlyObservableCollection<GameAction>(_undoActions);
		RedoActions = new ReadOnlyObservableCollection<GameAction>(_redoActions);

		UndoCommand = new SourceList<GameAction>(_undoActions.ToObservableChangeSet())
			.CountChanged
			.Select(x => x > 0)
			.ToCommand()
			.WithSubscribe(Undo);

		RedoCommand = new SourceList<GameAction>(_redoActions.ToObservableChangeSet())
			.CountChanged
			.Select(x => x > 0)
			.ToCommand()
			.WithSubscribe(Redo);

		IsDirty = _lastAppliedActionId
			.CombineLatest(_lastAppliedActionIdSaved)
			.Select(x => (x.First == null && x.Second == null) || x.First != x.Second)
			.ToReadOnlyReactiveProperty(default);
	}

	public void Initialize()
	{
		int? lastAppliedActionId = _database.Actions.GetLastAppliedActionId(_game.Id);
		int undoActionCountToLoad = ActionCountToLoad;

		if (lastAppliedActionId != null)
		{
			// We will load the last applied action and add it to the undo list.
			undoActionCountToLoad--;
		}

		LoadUndoActions(undoActionCountToLoad, lastAppliedActionId);
		LoadRedoActions(ActionCountToLoad, lastAppliedActionId);

		if (lastAppliedActionId != null)
		{
			// Load the last applied action separately as the undo and redo actions are all
			// before or after this one.
			GameAction lastAppliedAction = _database.Actions.Get(lastAppliedActionId.Value);
			_undoActions.Add(lastAppliedAction);
		}

		_lastAppliedActionId.Value = lastAppliedActionId;
		_lastAppliedActionIdSaved.Value = lastAppliedActionId;
	}

	public void Reset(Game game)
	{
		_game = game;
		_undoActions.Clear();
		_redoActions.Clear();

		_lastAppliedActionId.Value = null;
		_lastAppliedActionIdSaved.Value = null;
	}

	public void Save()
	{
		if (!_database.HasTransaction)
		{
			return;
		}

		_database.CommitTransaction();

		_lastAppliedActionIdSaved.Value = _lastAppliedActionId.Value;
	}

	public void Discard()
	{
		// TODO: This method does not revert to the last saved state. It only discards
		// the changes to the database.
		if (!_database.HasTransaction)
		{
			return;
		}

		_database.RollbackTransaction();
	}

	public void Apply(GameAction action)
	{
		action.Apply(_serviceProvider);

		GameAction? lastAppliedAction = _undoActions.LastOrDefault();

		_undoActions.Add(action);
		_redoActions.Clear();

		if (!_database.HasTransaction)
		{
			_database.BeginTransaction();
		}

		_database.Actions.DeleteAfter(lastAppliedAction, _game.Id);

		_database.Actions.Create(action, _game.Id);
		_database.Actions.UpdateLastAppliedAction(action, _game.Id);

		_lastAppliedActionId.Value = action.Id;

		Save();
	}

	private void Undo()
	{
		if (_undoActions.Count == 0)
		{
			return;
		}

		// This statement should be called when there is at least one action in the list.
		// If the list is empty, it means there are no more actions to load. The list is
		// empty at the initialization phase, but that case is handled separately.
		if (_undoActions.Count <= ActionCountToTriggerLoad)
		{
			LoadUndoActions(ActionCountToLoad, _undoActions[0].Id);
		}

		GameAction action = _undoActions[^1];
		
		_undoActions.Remove(action);
		_redoActions.Add(action);
		
		action.Undo(_serviceProvider);

		if (!_database.HasTransaction)
		{
			_database.BeginTransaction();
		}

		GameAction? lastAppliedAction = _undoActions.LastOrDefault();

		_database.Actions.UpdateLastAppliedAction(
			lastAppliedAction,
			_game.Id);

		_lastAppliedActionId.Value = lastAppliedAction?.Id;

		Save();
	}

	private void Redo()
	{
		if (_redoActions.Count == 0)
		{
			return;
		}

		// See the comment on similar statement inside undo method.
		if (_redoActions.Count <= ActionCountToTriggerLoad)
		{
			LoadRedoActions(ActionCountToLoad, _redoActions[0].Id);
		}

		GameAction action = _redoActions[^1];

		_redoActions.Remove(action);
		_undoActions.Add(action);
		
		action.Apply(_serviceProvider);

		if (!_database.HasTransaction)
		{
			_database.BeginTransaction();
		}

		_database.Actions.UpdateLastAppliedAction(
			action,
			_game.Id);

		_lastAppliedActionId.Value = action.Id;

		Save();
	}

	private void LoadUndoActions(int count, int? startActionId)
	{
		if (startActionId == null)
		{
			return;
		}
		
		List<GameAction> undoActions = _database.Actions.GetActionsBeforeId(
			_game.Id,
			startActionId.Value,
			count);

		if (undoActions.Count == 0)
		{
			return;
		}

		using (_undoActions.SuspendNotifications())
		{
			_undoActions.InsertRange(undoActions, 0);
		}
	}

	private void LoadRedoActions(int count, int? startActionId)
	{
		List<GameAction> redoActions = startActionId == null
			? _database.Actions.GetActions(_game.Id, count)
			: _database.Actions.GetActionsAfterId(_game.Id, startActionId.Value, count);

		if (redoActions.Count == 0)
		{
			return;
		}
		
		using (_redoActions.SuspendNotifications())
		{
			_redoActions.InsertRange(redoActions, 0);
		}
	}
}