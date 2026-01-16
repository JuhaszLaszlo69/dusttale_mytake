using Godot;

public partial class DiamondsFromBelow : Wave
{
    private PathFollow2D _pathFollow2D;
    private Timer _spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        // Right Below the battle box:
        GlobalPosition = new Vector2(656, GetViewport().GetVisibleRect().Size.Y);
        _pathFollow2D = GetNode<PathFollow2D>("%PathFollow2D");
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
            node2D.RotationDegrees += 180;
        }
    }

    private void _OnEndTimerTimeout()
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }
}
