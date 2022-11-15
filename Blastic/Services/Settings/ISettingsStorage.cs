namespace Blastic.Services.Settings
{
	/// <summary>
	/// An interface that provides CRUD operations for settings.
	/// </summary>
	public interface ISettingsStorage
	{
		/// <summary>
		/// Check if given key exists in store.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns>Whether the key exists in store.</returns>
		bool Contains(string key);

		/// <summary>
		/// Get the value corresponding to the given key. Returns the default value if key
		/// does not exist in store.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key to get corresponding value from store.</param>
		/// <returns>The value corresponding to the key.</returns>
		T? Get<T>(string key);

		/// <summary>
		/// Writes the given key and value to store.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Put<T>(string key, T value);

		/// <summary>
		/// Delete the given key and its value from store.
		/// </summary>
		/// <param name="key">The key to delete.</param>
		void Delete(string key);
	}
}