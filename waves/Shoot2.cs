using Godot;

public partial class Shoot2 : Wave
{
    private Timer _spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _spawnTimer.Timeout += _OnSpawnTimerTimeout;
    }

    private void _OnSpawnTimerTimeout()
    {
        Bullet instance = BulletScene.Instantiate() as Bullet;
        AddChild(instance);
        instance.GlobalPosition = new Vector2(GetViewport().GetVisibleRect().Size.X / 2, 0);
        instance.Rotate(Mathf.Pi);
    }
}
