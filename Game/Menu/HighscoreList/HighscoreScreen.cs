using Godot;
using Game.Utilities.Backend;

namespace Game.Menu.HighscoreList;

public partial class HighscoreScreen : Control
{
    private HttpRequest _personalScoreRequest;
    private HttpRequest _topPlayerRequest;

    public override void _Ready()
    {
        _personalScoreRequest = GetNode<HttpRequest>("Panel/PersonalScoreRequest");
        _topPlayerRequest = GetNode<HttpRequest>("Panel/TopPlayerRequest");


        _personalScoreRequest.Connect("request_completed", new Callable(this, nameof(OnPersonalScoreRequestCompleted)));
        _topPlayerRequest.Connect("request_completed", new Callable(this, nameof(OnTopPlayerRequestCompleted)));


        if (GameState.CurrentState == ConnectionState.Online)
        {
            var headers = new[]
            {
                "Content-Type: application/json",
                "Authorization: Bearer " + SecureStorage.LoadToken()
            };
            var err = _topPlayerRequest.Request(
                $"{Server.BaseUrl}/api/v1/protected/highscore/top",
                headers
            );

            if (err != Error.Ok)
                GD.PrintErr($"AuthRequest error: {err}");
        }
        else
        {
            GD.Print("Offline mode: No highscore data available.");
            // show offline message
        }
    }

    private void OnPersonalScoreRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode == 200)
        {
            GD.Print(result);
        }
        else
        {
            GD.PrintErr($"Error fetching personal score: {responseCode}");
        }
    }

    private void OnTopPlayerRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode == 200)
        {
            var bodyText = System.Text.Encoding.UTF8.GetString(body);
            var json = new Json();
            var parseErr = json.Parse(bodyText);
            if (parseErr == Error.Ok)
            {
                var scores = (Godot.Collections.Array)json.GetData();
                if (scores != null)
                {
                    var entryScene = GD.Load<PackedScene>("res://Menu/HighscoreList/entry.tscn");
                    var vbox = GetNode<VBoxContainer>("Panel/List/VBoxContainer");
                    vbox.ClearChildren();

                    for (int i = 0; i < scores.Count; i++)
                    {
                        var scoreDict = (Godot.Collections.Dictionary)scores[1];
                        if (scoreDict == null) continue;

                        var entry = entryScene.Instantiate<Control>();
                        entry.GetNode<Label>("Position").Text = (i + 1).ToString();
                        entry.GetNode<Label>("Name").Text = scoreDict["name"].ToString();
                        entry.GetNode<Label>("Score").Text = scoreDict["score"].ToString();
                        vbox.AddChild(entry);
                    }
                }
            }
            else
            {
                GD.PrintErr("Error parsing JSON response.");
            }
        }
        else
        {
            GD.PrintErr($"Error fetching top players: {responseCode}");
        }
    }
}

public static class NodeExtensions
{
    public static void ClearChildren(this Node node)
    {
        foreach (Node child in node.GetChildren())
            child.QueueFree();
    }
}