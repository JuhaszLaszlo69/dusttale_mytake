using Godot;

public partial class Wave : Node2D
{
    [Export]
    public Soul.Mode Mode { get; set; } = Soul.Mode.Red;
    
    [Export]
    public Vector2 BoxSize { get; set; } = new Vector2(0.5f, 1.0f);
    
    [Export]
    public float BoxSizeChangeTime { get; set; } = 0.3f;
    
    [Export]
    public PackedScene BulletScene { get; set; }

    private Timer _endTimer;

    public override void _Ready()
    {
        _endTimer = GetNodeOrNull<Timer>("EndTimer");
        if (_endTimer != null)
        {
            _endTimer.Timeout += OnEndTimerTimeout;
        }
    }

    private void OnEndTimerTimeout()
    {
        Node soul = GetTree().GetFirstNodeInGroup("soul");
        Global.Singleton.EmitSignal(Global.SignalName.WaveDone, this, soul);
    }

    protected virtual void OnSpawnTimerTimeout()
    {
        // Override in subclasses
    }
}
