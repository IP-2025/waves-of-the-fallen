// SecureStorage.cs
using Godot;
using System;

public partial class SecureStorage : Node
{
	private const string TokenPath = "user://token.txt";
	private string _cachedToken = null;

	public bool SaveToken(string token)
	{
		var file = new File();
		var err = file.Open(TokenPath, File.ModeFlags.Write);
		if (err != Error.Ok)
		{
			GD.PrintErr($"SecureStorage: Cannot open to write: {err}");
			return false;
		}
		file.StoreString(token);
		file.Close();
		_cachedToken = token;
		return true;
	}

	public string LoadToken()
	{
		if (_cachedToken != null)
			return _cachedToken;

		var file = new File();
		if (!file.FileExists(TokenPath))
			return null;

		var err = file.Open(TokenPath, File.ModeFlags.Read);
		if (err != Error.Ok)
		{
			GD.PrintErr($"SecureStorage: Cannot open to read: {err}");
			return null;
		}
		var token = file.GetAsText().Trim();
		file.Close();
		_cachedToken = token;
		return token;
	}

	public bool DeleteToken()
	{
		var file = new File();
		if (file.FileExists(TokenPath))
		{
			var err = file.Remove(TokenPath);
			if (err != Error.Ok)
			{
				GD.PrintErr($"SecureStorage: Failed to delete: {err}");
				return false;
			}
		}
		_cachedToken = null;
		return true;
	}

}
