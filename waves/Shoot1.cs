using Godot;

public partial class Shoot1 : Wave
{
    private PathFollow2D _pathFollow2D;
    private Label _instructions;
    private Timer _spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        GlobalPosition = new Vector2(650, 0);
        _pathFollow2D = GetNode<PathFollow2D>("%PathFollow2D");
        _instructions = GetNode<Label>("%Instructions");
        _instructions.GlobalPosition = GetViewport().GetVisibleRect().Size / 2 + new Vector2(-_instructions.Size.X, _instructions.Size.Y) / 2;
        _instructions.Text = Util.Shake(_instructions.Text);
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _spawnTimer.Timeout += _OnSpawnTimerTimeout;
    }

    private void _OnSpawnTimerTimeout()
    {
        Node bullet = BulletScene.Instantiate();
        AddChild(bullet);
        _pathFollow2D.ProgressRatio = GD.Randf();
        if (bullet is Node2D node2D)
        {
            node2D.GlobalTransform = _pathFollow2D.GlobalTransform;
        }
        if (bullet is Bullet bulletInstance)
        {
            bulletInstance.Speed = 500;
        }
    }

    private void _OnEndTimerTimeout()
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }
}
