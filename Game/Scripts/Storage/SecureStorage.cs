#nullable disable
using Godot;
using System;

public static class SecureStorage
{
	private const string TokenFileName = "token.txt";
	private const string TokenPath     = "user://" + TokenFileName;
	private static string _cachedToken;

	public static bool SaveToken(string token)
	{
		try
		{
			using var file = FileAccess.Open(TokenPath, FileAccess.ModeFlags.Write);
			file.StoreString(token);
			_cachedToken = token;
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr($"SecureStorage: write error: {e.Message}");
			return false;
		}
	}

	public static string LoadToken()
	{
		if (_cachedToken != null)
			return _cachedToken;

		if (!FileAccess.FileExists(TokenPath))
			return null;  // null is allowed because nullable is disabled

		try
		{
			using var file = FileAccess.Open(TokenPath, FileAccess.ModeFlags.Read);
			var token = file.GetAsText().Trim();
			_cachedToken = token;
			return token;
		}
		catch (Exception e)
		{
			GD.PrintErr($"SecureStorage: read error: {e.Message}");
			return null;
		}
	}

	public static bool DeleteToken()
	{
		try
		{
			var dir = DirAccess.Open("user://");
			if (dir.FileExists(TokenFileName))
			{
				var err = dir.Remove(TokenFileName);
				if (err != Error.Ok)
				{
					GD.PrintErr($"SecureStorage: delete error: {err}");
					return false;
				}
			}
			_cachedToken = null;
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr($"SecureStorage: delete exception: {e.Message}");
			return false;
		}
	}
}
