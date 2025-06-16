namespace Game.Utilities.Backend
{
	public static class ServerConfig
	{
	#if DEBUG
		public const string BaseUrl = "http://localhost:3000";
	#else
		public const string BaseUrl = "http://waves-of-the-fallen.duckdns.org:32424";
	#endif
	}
}
