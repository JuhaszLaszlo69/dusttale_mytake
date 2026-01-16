using Godot;

public partial class Shoot3 : Wave
{
    private PackedScene _whiteBulletScene = GD.Load<PackedScene>("uid://vy7eelm7fbgd");

    private int _counter = 0;
    private Timer _spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _spawnTimer.Timeout += _OnSpawnTimerTimeout;
    }

    private void _OnSpawnTimerTimeout()
    {
        float center = GetViewport().GetVisibleRect().Size.X / 2;
        float centerOffset = 20;
        _counter += 1;
        bool even = _counter % 2 == 0;
        
        LinearBullet blueBullet = ((LinearBullet)BulletScene.Instantiate()).New(Mathf.Pi);
        AddChild(blueBullet);
        blueBullet.GlobalPosition = new Vector2(center + (even ? centerOffset : -centerOffset), 0);
        
        LinearBullet whiteBullet = ((LinearBullet)_whiteBulletScene.Instantiate()).New(0);
        AddChild(whiteBullet);
        whiteBullet.GlobalPosition = new Vector2(center + (even ? -centerOffset : centerOffset), 0);
    }
}
