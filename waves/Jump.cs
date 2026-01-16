using Godot;

public partial class Jump : Wave
{
    public override void _Ready()
    {
        base._Ready();
        OnSpawnTimerTimeout();
    }

    protected override void OnSpawnTimerTimeout()
    {
        if (BulletScene != null)
        {
            JumpObstacle bullet = BulletScene.Instantiate<JumpObstacle>();
            AddChild(bullet);
            Vector2 screenSize = DisplayServer.ScreenGetSize();
            bullet.GlobalPosition = new Vector2(screenSize.X, JumpObstacle.BattleBoxBottom);
        }
    }
}
