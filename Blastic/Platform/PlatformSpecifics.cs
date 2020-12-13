namespace Blastic.Platform
{
	public static class PlatformSpecifics
	{
		/// <summary>
		/// Singleton property for the current platform.
		/// </summary>
		public static IPlatformSpecifics Current { get; set; } = new DefaultPlatformSpecifics();
	}
}