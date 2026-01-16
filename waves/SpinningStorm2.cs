using Godot;
using System.Linq;

public partial class SpinningStorm2 : Wave
{
    private Node2D _spawners;
    private Timer _spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        // in the middle of the battle box:
        GlobalPosition = new Vector2(950, 570);
        _spawners = GetNode<Node2D>("%Spawners");
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _spawnTimer.Timeout += _OnSpawnTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        RotationDegrees += (float)delta * 100;
    }

    private void _OnSpawnTimerTimeout()
    {
        foreach (Node2D spawner in _spawners.GetChildren().Cast<Node2D>())
        {
            Node instance = BulletScene.Instantiate();
            Global.Singleton.EmitSignal(Global.SignalName.AddBullet, instance, spawner.GlobalTransform);
        }
    }

    private void _OnEndTimerTimeout()
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }
}
