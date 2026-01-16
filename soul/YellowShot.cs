using Godot;

public partial class YellowShot : Area2D
{
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += (float)delta * 400 * Vector2.Up;
    }

    private void _OnAreaEntered(Area2D area)
    {
        if (!(area is Bullet bullet) || !bullet.Shootable)
        {
            return;
        }
        Global.Singleton.EmitSignal(Global.SignalName.BulletDestroyed, area.GlobalPosition);
        area.QueueFree();
        QueueFree();
    }
}
