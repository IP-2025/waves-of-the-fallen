using Godot;

public partial class RegisterScreen : Control
{
	private static readonly string REGISTER_URL = $"{Game.Scripts.Config.Server.BaseUrl}/api/v1/auth/register";

	private LineEdit _emailField;
	private LineEdit _usernameField;
	private LineEdit _passwordField;
	private Button _registerButton;
	private Button _backButton;
	private HttpRequest _registerRequest;
	private Label _errorLabel;

	public override void _Ready()
	{
		_emailField = GetNode<LineEdit>("Panel/EmailField");
		_usernameField = GetNode<LineEdit>("Panel/NameField");
		_passwordField = GetNode<LineEdit>("Panel/PasswordField");
		_registerButton = GetNode<Button>("Panel/RegisterButton");
		_backButton = GetNode<Button>("Panel/BackButton");
		_registerRequest = GetNode<HttpRequest>("Panel/RegisterRequest");
		_errorLabel = GetNode<Label>("Panel/ErrorLabel");

		_registerButton.Connect("pressed", new Callable(this, nameof(OnRegisterButtonPressed)));
		_backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
		_registerRequest.Connect("request_completed", new Callable(this, nameof(OnRegisterRequestCompleted)));

	}

	private void OnRegisterButtonPressed()
	{
		GD.Print("Button pressed!");
		SoundManager.Instance.PlayUI();
		var username = _usernameField.Text;
		var email = _emailField.Text.Trim();
		var password = _passwordField.Text;

		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
		{
			ShowError("Please fill in all fields.");
			return;
		}
		
		if (!IsValidEmail(email))
		{
			ShowError("Please enter a valid email address.");
			return;
		}	

		var body = Json.Stringify(
			new Godot.Collections.Dictionary
			{
				{ "email", email },
				{ "username", username },
				{ "password", password }
			}
		);
		
		var headers = new[] { "Content-Type: application/json" };
		var err = _registerRequest.Request(
			REGISTER_URL,
			headers,
			HttpClient.Method.Post,
			body
		);
		
		GD.Print("Request sent!");

		if (err != Error.Ok)
			ShowError($"Request error: {err}");
		else
			_registerButton.Disabled = true;
	}

	private void OnBackButtonPressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Login/login_screen.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnRegisterRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_registerButton.Disabled = false;

		if (responseCode == 201)
		{
			var bodyText = System.Text.Encoding.UTF8.GetString(body);

			var json = new Json();
			var parseErr = json.Parse(bodyText);
			if (parseErr == Error.Ok)
			{
				var response = json.GetData().AsGodotDictionary();
				OnRegisterSuccess(response);
				return;
			}

			ShowError("Unexpected server response.");
		}
		else if (responseCode == 409)
		{
			ShowError("User already exists.");
		}
		else
		{
			ShowError($"Server error: {responseCode} Internal Server Error");
		}
	}

	private void OnRegisterSuccess(Godot.Collections.Dictionary data)
	{
		GD.Print("Login successful!");
		
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Login/login_screen.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		GetTree().ChangeSceneToPacked(scene);
	}

	private void ShowError(string message)
	{
		GD.PrintErr("Error: " + message);
		
		_errorLabel.Text = message;
		_errorLabel.Visible = true;
	}
	
	private bool IsValidEmail(string email)
	{
		var emailRegex = new System.Text.RegularExpressions.Regex(
			@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
			System.Text.RegularExpressions.RegexOptions.Compiled
		);
		return emailRegex.IsMatch(email);
	}
}
