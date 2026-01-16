using Godot;

public partial class FollowSoul : Wave
{
    private Timer _timer;

    public override void _Ready()
    {
        base._Ready();
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Node bullet = BulletScene.Instantiate();
        
        AddChild(bullet);
        if (bullet is Node2D node2D && soul != null)
        {
            node2D.GlobalPosition = soul.GlobalPosition + new Vector2(0, 100);
        }
        
        _timer = GetNode<Timer>("EndTimer");
        _timer.Timeout += _OnTimerTimeout;
    }

    private void _OnTimerTimeout()
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }
}
