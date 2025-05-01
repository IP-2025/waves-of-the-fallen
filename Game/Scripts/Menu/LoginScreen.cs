using Godot;
using System;
using Godot.Collections;
using WavesOfTheFallen;

public partial class LoginScreen : Control
{
	private static readonly string LOGIN_URL = $"{Config.BaseUrl}/api/v1/auth/login";

	private LineEdit _emailField;
	private LineEdit _passwordField;
	private Button _loginButton;
	private Button _offlineButton;
	private HttpRequest _httpRequest;
	private Label _errorLabel;

	public override void _Ready()
	{
		_emailField = GetNode<LineEdit>("Panel/EmailField");
		_passwordField = GetNode<LineEdit>("Panel/PasswordField");
		_loginButton = GetNode<Button>("Panel/LoginButton");
		_offlineButton = GetNode<Button>("Panel/OfflineButton");
		_httpRequest = GetNode<HttpRequest>("Panel/HttpRequest");
		_errorLabel = GetNode<Label>("Panel/ErrorLabel");

		_loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));
		_offlineButton.Connect("pressed", new Callable(this, nameof(OnOfflineButtonPressed)));
		_httpRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));
		
		_errorLabel.Visible = false;
	}

	private void OnOfflineButtonPressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnLoginButtonPressed()
	{
		string email = _emailField.Text.Trim();
		string password = _passwordField.Text;

		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			ShowError("Please fill in both fields.");
			return;
		}

		string body = Json.Stringify(new Godot.Collections.Dictionary
		{
			{ "email", email },
			{ "password", password }
		});

		// Send POST request
		var headers = new[] { "Content-Type: application/json" };
		var err = _httpRequest.Request(
			LOGIN_URL,
			headers,
			Godot.HttpClient.Method.Post,
			body
		);

		if (err != Error.Ok)
			ShowError($"Request error: {err}");
		else
			_loginButton.Disabled = true;
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_loginButton.Disabled = false;

		if (responseCode == 200)
		{
			string bodyText = System.Text.Encoding.UTF8.GetString(body);

		 	var json = new Json();
		 	var parseErr = json.Parse(bodyText);
		 	if (parseErr == Error.Ok)
		 	{
				var response = json.GetData().AsGodotDictionary();
		 		OnLoginSuccess(response);
		 		return;
		 	}

		 	ShowError("Unexpected server response.");
		 }
		 else if (responseCode == 401)
		 {
		 	ShowError("Invalid credentials.");
		 }
		 else
		 {
		 	ShowError($"Server error: {responseCode}");
		 }
	}

	private void OnLoginSuccess(Godot.Collections.Dictionary data)
	{
		GD.Print("Login successful!");
		GD.Print("Token: " + data["token"].ToString());
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void ShowError(string message)
	{
		GD.PrintErr("Error: " + message);
		
		_errorLabel.Text = message;
		_errorLabel.Visible = true;
	}
}
