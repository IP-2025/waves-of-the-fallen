using Godot;
using Game.Scripts.Config;

public partial class LoginScreen : Control
{
	private const string LoginUrl = $"{Server.BaseUrl}/api/v1/auth/login";

	private LineEdit _emailField;
	private LineEdit _passwordField;
	private Button _loginButton;
	private Button _offlineButton;
	private HttpRequest _httpRequest;
	private HttpRequest _authRequest;
	private Label _errorLabel;
	private Button _registerButton;
	
	public override void _Ready()
	{
		_emailField = GetNode<LineEdit>("Panel/EmailField");
		_passwordField = GetNode<LineEdit>("Panel/PasswordField");
		_loginButton = GetNode<Button>("Panel/LoginButton");
		_offlineButton = GetNode<Button>("Panel/OfflineButton");
		_httpRequest = GetNode<HttpRequest>("Panel/LoginRequest");
		_authRequest = GetNode<HttpRequest>("Panel/AuthRequest");
		_errorLabel = GetNode<Label>("Panel/ErrorLabel");
		_registerButton = GetNode<Button>("Panel/RegisterButton");

		_loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));
		_offlineButton.Connect("pressed", new Callable(this, nameof(OnOfflineButtonPressed)));
		_httpRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));
		_authRequest.Connect("request_completed",   new Callable(this, nameof(OnAuthRequestCompleted)));
		_registerButton.Connect("pressed", new Callable(this, nameof(OnRegisterButtonPressed)));

		
		_errorLabel.Visible = false;
		
		var token = SecureStorage.LoadToken();
		if (string.IsNullOrEmpty(token)) return;
		const string url = $"{Server.BaseUrl}/api/v1/protected/";
		var headers = new[] { $"Authorization: Bearer {token}" };
		var err = _authRequest.Request(url, headers, Godot.HttpClient.Method.Get);
		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}
	
	private void OnAuthRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		// If our saved token is still valid, go straight to main menu:
		if (responseCode == 200)
		{
			var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
			GetTree().ChangeSceneToPacked(scene);
		}
		else
		{
			// invalid/expired token → stay on login screen
			SecureStorage.DeleteToken();
		}
	}

	private void OnOfflineButtonPressed()
	{
		GameState.CurrentState = ConnectionState.Offline;
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnLoginButtonPressed()
	{
		var email = _emailField.Text.Trim();
		var password = _passwordField.Text;

		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			ShowError("Please fill in both fields.");
			return;
		}

		var body = Json.Stringify(new Godot.Collections.Dictionary
		{
			{ "email", email },
			{ "password", password }
		});

		// Send POST request
		var headers = new[] { "Content-Type: application/json" };
		var err = _httpRequest.Request(
			LoginUrl,
			headers,
			HttpClient.Method.Post,
			body
		);

		if (err != Error.Ok)
			ShowError($"Request error: {err}");
		else
			_loginButton.Disabled = true;
	}

	private void OnRegisterButtonPressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/register_screen.tscn");
		if (scene == null) GD.PrintErr("Register Scene not found");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_loginButton.Disabled = false;

		switch (responseCode)
		{
			case 200:
			{
				var bodyText = System.Text.Encoding.UTF8.GetString(body);

				var json = new Json();
				var parseErr = json.Parse(bodyText);
				if (parseErr == Error.Ok)
				{
					var response = json.GetData().AsGodotDictionary();
					OnLoginSuccess(response);
					return;
				}

				ShowError("Unexpected server response.");
				break;
			}
			case 401:
				ShowError("Invalid credentials.");
				break;
			default:
				ShowError($"Server error: {responseCode}");
				break;
		}
	}

	private void OnLoginSuccess(Godot.Collections.Dictionary data)
	{
		GD.Print("Login successful!");
		GD.Print("Token: " + data["token"].ToString());
		
		var prevToken = SecureStorage.LoadToken();
		if(!string.IsNullOrEmpty(prevToken)){
			SecureStorage.DeleteToken();
		}
		SecureStorage.SaveToken(data["token"].ToString());
		
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
