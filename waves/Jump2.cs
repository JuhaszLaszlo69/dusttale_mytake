using Godot;

public partial class Jump2 : Wave
{
    public override void _Ready()
    {
        base._Ready();
        _OnSpawnTimerTimeout();
    }

    private void _OnSpawnTimerTimeout()
    {
        JumpObstacle bullet1 = BulletScene.Instantiate() as JumpObstacle;
        AddChild(bullet1);
        bullet1.GlobalPosition = new Vector2(0, JumpObstacle.BattleBoxBottom);
        bullet1.Dir = Vector2.Right;
        
        JumpObstacle bullet2 = BulletScene.Instantiate() as JumpObstacle;
        AddChild(bullet2);
        bullet2.GlobalPosition = new Vector2(GetViewport().GetVisibleRect().Size.X, JumpObstacle.BattleBoxBottom);
        bullet2.Dir = Vector2.Left;
    }

    private void _OnEndTimerTimeout()
    {
        Soul soul = GetTree().GetFirstNodeInGroup("soul") as Soul;
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }
}
