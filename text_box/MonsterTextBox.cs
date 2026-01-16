using Godot;
using System.Threading.Tasks;

public partial class MonsterTextBox : RichTextLabel
{
    [Signal]
    public delegate void PlayMonsterSpeakAnimEventHandler();

    private float _scrollSpeed = 0.05f;

    private Timer _timer;
    private AudioStreamPlayer _textSound;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("%Timer");
        _textSound = GetNode<AudioStreamPlayer>("%TextSound");
    }

    public async Task Speak(string newText)
    {
        await StopTalking();
        Text = newText;
        VisibleCharacters = 0;
        _timer.Start();
        _textSound.Play();
        EmitSignal(SignalName.PlayMonsterSpeakAnim);
    }

    private void _OnTimerTimeout()
    {
        if (VisibleRatio == 1.0f)
        {
            _timer.Stop();
            return;
        }
        VisibleCharacters += 1;
        _textSound.Play();
        _timer.WaitTime = _scrollSpeed;
    }

    public async Task StopTalking()
    {
        _timer.Stop();
        VisibleRatio = 0.0f;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
