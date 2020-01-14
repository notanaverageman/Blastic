namespace Blastic.Platform
{
	public static class PlatformSpecifics
	{
		public static IPlatformSpecifics Current { get; set; } = new DefaultPlatformSpecifics();
	}
}