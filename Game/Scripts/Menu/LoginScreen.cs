using Godot;
using System;
using Godot.Collections;

public partial class LoginScreen : Control
{
	// API endpoint
	private const string LOGIN_URL = "https://localhost:3000/api/v1/auth/login";

	// UI nodes
	private LineEdit _emailField;
	private LineEdit _passwordField;
	private Button _loginButton;
	private HttpRequest _httpRequest;

	public override void _Ready()
	{
		// Get node references
		_emailField = GetNode<LineEdit>("Panel/EmailField");
		_passwordField = GetNode<LineEdit>("Panel/PasswordField");
		_loginButton = GetNode<Button>("Panel/LoginButton");
		_httpRequest = GetNode<HttpRequest>("Panel/HttpRequest");

		// Connect signals using Callable
		_loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));
		_httpRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));
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

		// Prepare JSON payload
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

		GD.Print($"Request completed with response code: {responseCode}");
		GD.Print($"Response body: {System.Text.Encoding.UTF8.GetString(body)}");

		// if (responseCode == 200)
		// {
		// 	string bodyText = System.Text.Encoding.UTF8.GetString(body);

		// 	// Parse JSON response
		// 	var json = new Json();
		// 	var parseErr = json.Parse(bodyText);
		// 	if (parseErr == Error.Ok
		// 		&& data.ContainsKey("token"))
		// 	{
		// 		OnLoginSuccess(null);
		// 		return;
		// 	}

		// 	ShowError("Unexpected server response.");
		// }
		// else if (responseCode == 401)
		// {
		// 	ShowError("Invalid credentials.");
		// }
		// else
		// {
		// 	ShowError($"Server error: {responseCode}");
		// }
	}

	private void OnLoginSuccess(Dictionary data)
	{
		//string token = data["token"].ToString();
		//GD.Print($"Login successful, token: {token}");
		// Store token as needed, then:
		GetTree().ChangeSceneToFile("res://Scenes/mainMenu.tscn");
	}

	private void ShowError(string message)
	{
		GD.PrintErr("Error: " + message);
		// TODO: display this message in a Label on-screen
	}
}
