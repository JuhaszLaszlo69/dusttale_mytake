using Godot;
using Godot.Collections;

public partial class TitleScreen : CanvasLayer
{
    [Export]
    public Array<PackedScene> Enemies { get; set; } = new Array<PackedScene>();

    private RichTextLabel _title;
    private VBoxContainer _battlesContainer;
    private AudioStreamPlayer _song;
    private AudioStreamPlayer _moveSound;
    private AudioStreamPlayer _encounter1;
    private AudioStreamPlayer _encounter2;

    public override void _Ready()
    {
        _title = GetNode<RichTextLabel>("%Title");
        _battlesContainer = GetNode<VBoxContainer>("%BattlesContainer");
        _song = GetNode<AudioStreamPlayer>("%Song");
        _moveSound = GetNode<AudioStreamPlayer>("%MoveSound");
        _encounter1 = GetNode<AudioStreamPlayer>("%Encounter1");
        _encounter2 = GetNode<AudioStreamPlayer>("%Encounter2");
        
        FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
        fade.FadeFromBlack();
        _title.Text = Util.Shake(_title.Text);
        
        // Add overworld button first
        Godot.Button overworldButton = new Godot.Button();
        overworldButton.AddThemeFontSizeOverride("font_size", 50);
        overworldButton.Text = "Go to Overworld";
        _battlesContainer.AddChild(overworldButton);
        overworldButton.PivotOffset = overworldButton.Size / 2;
        overworldButton.FocusEntered += () => _OnFocusEntered(overworldButton);
        overworldButton.FocusExited += () => _OnFocusExited(overworldButton);
        overworldButton.Pressed += GoToOverworld;
        
        // Add debug button to view cutscene
        Godot.Button debugButton = new Godot.Button();
        debugButton.AddThemeFontSizeOverride("font_size", 50);
        debugButton.Text = "[DEBUG] View Cutscene";
        _battlesContainer.AddChild(debugButton);
        debugButton.PivotOffset = debugButton.Size / 2;
        debugButton.FocusEntered += () => _OnFocusEntered(debugButton);
        debugButton.FocusExited += () => _OnFocusExited(debugButton);
        debugButton.Pressed += GoToCutscene;
        
        foreach (PackedScene scene in Enemies)
        {
            Node instance = scene.Instantiate();
            if (!(instance is Enemy))
            {
                throw new System.Exception("Only put Enemy scenes in the enemies array");
            }
            Enemy enemy = instance as Enemy;
            Godot.Button button = new Godot.Button();
            button.AddThemeFontSizeOverride("font_size", 50);
            button.Text = $"[DEBUG] - {enemy.EnemyName}";
            _battlesContainer.AddChild(button);
            button.PivotOffset = button.Size / 2;
            button.FocusEntered += () => _OnFocusEntered(button);
            button.FocusExited += () => _OnFocusExited(button);
            button.Pressed += () => GoToBattle(enemy);
        }
        
        if (_battlesContainer.GetChild(0) is Godot.Button firstButton)
        {
            firstButton.GrabFocus();
        }
    }

    private async void GoToOverworld()
    {
        _song.Stop();
        FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
        await fade.FadeIntoBlack();
        
        // Try loading the scene first to check if it exists
        PackedScene scene = GD.Load<PackedScene>("res://maps/overworld_original.tscn");
        if (scene == null)
        {
            GD.Print("ERROR: Could not load overworld_original.tscn");
            // Try alternative
            scene = GD.Load<PackedScene>("res://overworld.tscn");
            if (scene == null)
            {
                GD.Print("ERROR: Could not load overworld.tscn either!");
                return;
            }
        }
        
        Error error = GetTree().ChangeSceneToFile("res://maps/overworld_original.tscn");
        if (error != Error.Ok)
        {
            GD.Print("Error changing scene: ", error);
            GD.Print("Error code: ", error);
        }
    }

    private void _OnFocusEntered(Godot.Button button)
    {
        button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 1.0f);
        _moveSound.Play();
        Tween tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.SetTrans(Tween.TransitionType.Bounce);
        tween.TweenProperty(button, "scale", new Vector2(1.5f, 1.5f), 0.2);
        tween.TweenProperty(button, "scale", new Vector2(1.0f, 1.0f), 0.1);
    }

    private void _OnFocusExited(Godot.Button button)
    {
        button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 0.5f);
    }

    private async void GoToCutscene()
    {
        _song.Stop();
        FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
        await fade.FadeIntoBlack();
        if (IsInsideTree() && GetTree() != null)
        {
            GetTree().ChangeSceneToFile("res://cutscenes/all_bosses_defeated.tscn");
        }
    }

    private async void GoToBattle(Enemy enemy)
    {
        _song.Stop();
        _encounter1.Play();
        string debugText = $"[DEBUG] - {enemy.EnemyName}";
        foreach (Node child in _battlesContainer.GetChildren())
        {
            if (child is Godot.Button button)
            {
                button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, button.Text == debugText ? 1.0f : 0.0f);
                button.ReleaseFocus();
            }
        }
        await ToSignal(GetTree().CreateTimer(0.25), Timer.SignalName.Timeout);
        _encounter2.Play();
        FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
        await fade.FadeIntoBlack();
        Battle.Enemy = enemy;
        GetTree().ChangeSceneToFile("uid://45qmet5s5aix");
    }
}
