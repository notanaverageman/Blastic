using System.Threading;
using System.Threading.Tasks;

namespace Blastic.Services.Settings
{
	/// <summary>
	/// An interface that provides CRUD operations for setting storage.
	/// </summary>
	public interface ISettingsService
	{
		/// <summary>
		/// Check if given key exists in store.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Whether the key exists in store.</returns>
		Task<bool> Contains(
			string key,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the value corresponding to the given key. Returns the default value if key
		/// does not exist in store.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key to get corresponding value from store.</param>
		/// <param name="defaultValue">Default value to return if key does not exist in store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The value corresponding to the key.</returns>
		Task<T> Get<T>(
			string key,
			T defaultValue = default,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Writes the given key and value to store.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task to be awaited.</returns>
		Task Put<T>(
			string key,
			T value,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete the given key and its value from store.
		/// </summary>
		/// <param name="key">The key to delete.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task to be awaited.</returns>
		Task Delete(
			string key,
			CancellationToken cancellationToken = default);
	}
}