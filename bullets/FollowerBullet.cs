using Godot;

public partial class FollowerBullet : Bullet
{
    public override void _PhysicsProcess(double delta)
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        if (soul == null || !IsInstanceValid(soul))
        {
            return;
        }
        Vector2 dir = GlobalPosition.DirectionTo(soul.GlobalPosition);
        GlobalPosition += (float)delta * dir * Speed;
    }
}
