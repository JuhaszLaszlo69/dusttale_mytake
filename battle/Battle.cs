using Godot;
using Godot.Collections;
using System.Threading.Tasks;

public partial class Battle : Node2D
{
    public static int BattleCounter { get; set; } = -1;
    public static Enemy Enemy { get; set; }

    // Battle state variables
    public bool GonnaAttack { get; set; } = false;
    public bool GonnaAct { get; set; } = false;
    public bool IsChoosingAct { get; set; } = false;
    public bool IsReadingActText { get; set; } = false;
    public bool IsReadingItemText { get; set; } = false;
    public bool IsAttacking { get; set; } = false;
    public bool GonnaSpare { get; set; } = false;
    public bool IsChoosingItem { get; set; } = false;
    public bool BattleWon { get; set; } = false;
    public bool BattleLost { get; set; } = false;
    public bool CanSpare { get; set; } = false;
    public bool MonsterSpeaking { get; set; } = false;
    public int TurnCounter { get; set; } = 0;

    private int _playerHp = 20;
    public int PlayerHp
    {
        get => _playerHp;
        set
        {
            _playerHp = Mathf.Clamp(value, 0, 20);
            _hpBar.Value = _playerHp;
            _hp2.Text = $"{_playerHp} / 20";
        }
    }

    public int EnemyHp { get; set; } = 500;
    public string EnemyName { get; set; } = "Name here";
    public string EncounterText { get; set; } = "* name here drew new !";
    public string IdleText { get; set; } = "* name here is staring at you angerly";
    public string MonsterText { get; set; }

    private int _enemyMercy = 0;
    public int EnemyMercy
    {
        get => _enemyMercy;
        set
        {
            _enemyMercy = value;
            if (_enemyMercy >= 100)
            {
                CanSpare = true;
            }
        }
    }

    public Array<string> Acts { get; set; } = new Array<string>();
    [Export]
    public Array<Item> Items { get; set; } = new Array<Item>();
    public Array<PackedScene> BulletWaves { get; set; } = new Array<PackedScene>();

    private PackedScene _button = GD.Load<PackedScene>("uid://ptt71q0lsxgx");

    private TextBox _textBox;
    private MonsterTextBox _monsterTextBox;
    private TextureProgressBar _hpBar;
    private Label _hp2;
    private AudioStreamPlayer _music;
    private Node2D _attackBar;
    private Control _attackLine;
    private CanvasItem _speechBox;
    private Label _damage;
    private Node2D _monster;
    private Sprite2D _monsterSprite;
    private AnimationPlayer _anim;
    private AudioStreamPlayer _selectSound;
    private AudioStreamPlayer _useItemSound;
    private AudioStreamPlayer _soulHitSound;
    private AudioStreamPlayer _monsterHurtSound;
    private AudioStreamPlayer _battleDone;
    private AudioStreamPlayer _shootSound;
    private AudioStreamPlayer _soulBreak;
    private AudioStreamPlayer _knifeSlashSound;
    private Node2D _bullets;
    private AnimatedSprite2D _knife;
    private Control _box;
    private HBoxContainer _buttonsContainer;
    private GridContainer _optionsContainer;
    private Godot.Button _attackButton;
    private Godot.Button _actButton;
    private Godot.Button _itemButton;
    private Godot.Button _mercyButton;
    private Timer _uiCooldownTimer;
    private AudioStreamPlayer _moveSound;

    public override async void _Ready()
    {
        // Get node references
        _textBox = GetNode<TextBox>("%TextBox");
        _monsterTextBox = GetNode<MonsterTextBox>("%MonsterTextBox");
        _hpBar = GetNode<TextureProgressBar>("%HPBar");
        _hp2 = GetNode<Label>("%HP2");
        _music = GetNode<AudioStreamPlayer>("%Music");
        _attackBar = GetNode<Node2D>("%AttackBar");
        _attackLine = GetNode<Control>("%AttackLine");
        _speechBox = GetNode<CanvasItem>("%SpeechBox");
        _damage = GetNode<Label>("Damage");
        _monster = GetNode<Node2D>("%Monster");
        _monsterSprite = GetNode<Sprite2D>("%MonsterSprite");
        _anim = GetNode<AnimationPlayer>("%Anim");
        _selectSound = GetNode<AudioStreamPlayer>("%SelectSound");
        _useItemSound = GetNode<AudioStreamPlayer>("%UseItemSound");
        _soulHitSound = GetNode<AudioStreamPlayer>("%SoulHitSound");
        _monsterHurtSound = GetNode<AudioStreamPlayer>("%MonsterHurtSound");
        _battleDone = GetNode<AudioStreamPlayer>("%BattleDone");
        _shootSound = GetNode<AudioStreamPlayer>("%ShootSound");
        _soulBreak = GetNode<AudioStreamPlayer>("%SoulBreak");
        _knifeSlashSound = GetNode<AudioStreamPlayer>("%KnifeSlashSound");
        _bullets = GetNode<Node2D>("Bullets");
        _knife = GetNode<AnimatedSprite2D>("%Knife");
        _box = GetNode<Control>("%Box");
        _buttonsContainer = GetNode<HBoxContainer>("%ButtonsContainer");
        _optionsContainer = GetNode<GridContainer>("%OptionsContainer");
        _attackButton = GetNode<Godot.Button>("%AttackButton");
        _actButton = GetNode<Godot.Button>("%ActButton");
        _itemButton = GetNode<Godot.Button>("%ItemButton");
        _mercyButton = GetNode<Godot.Button>("%MercyButton");
        _uiCooldownTimer = GetNode<Timer>("%UiCooldownTimer");
        _moveSound = GetNode<AudioStreamPlayer>("%MoveSound");

        FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
        await fade.FadeFromBlack();

        Array<AudioStream> songs = new Array<AudioStream>
        {
            GD.Load<AudioStream>("uid://dpexiickfpwht"),
            GD.Load<AudioStream>("uid://b5k27ym6e01c6")
        };

        GD.Randomize();
        BattleCounter += 1;
        _music.Stream = songs[BattleCounter % songs.Count];
        _music.Play();

        Array<CanvasItem> makeMeTransparent = new Array<CanvasItem> { (CanvasItem)_attackBar, (CanvasItem)_attackLine, _speechBox, _damage };
        foreach (CanvasItem ui in makeMeTransparent)
        {
            ui.Show();
            ui.Modulate = new Color(ui.Modulate.R, ui.Modulate.G, ui.Modulate.B, 0.0f);
        }

        // Set up enemy
        _monster.AddChild(Enemy);
        _monsterSprite.Texture = Enemy.Sprite;
        _monsterSprite.Scale *= Enemy.SpriteScale;
        _damage.GlobalPosition = MonsterPosition();
        EnemyName = Enemy.EnemyName;
        EnemyHp = Enemy.HP;
        Acts = Enemy.Acts.Duplicate(true);
        BulletWaves = Enemy.BulletWaves.Duplicate(true);
        EncounterText = Enemy.EncounterText;
        _textBox.Scroll(EncounterText);

        // Load items from global inventory
        Items = Global.Singleton.battleInventory.Duplicate(true);

        Global.Singleton.WaveDone += FinishHell;
        Global.Singleton.AddBullet += (Node2D bullet, Transform2D transform) =>
        {
            _bullets.AddChild(bullet);
            bullet.GlobalTransform = transform;
        };
        Global.Singleton.ChangeMercy += (int amount) =>
        {
            EnemyMercy += amount;
        };
        Global.Singleton.HealPlayer += (int amount) =>
        {
            PlayerHp += amount;
            _useItemSound.Play();
        };
        Global.Singleton.BulletDestroyed += (Vector2 pos) =>
        {
            PackedScene bulletParticle = GD.Load<PackedScene>("uid://jciddngihwq7");
            Node2D instance = bulletParticle.Instantiate<Node2D>();
            instance.GlobalPosition = pos;
            AddChild(instance);
        };
        _monsterTextBox.PlayMonsterSpeakAnim += MonsterSpeakingAnim;
        Global.Singleton.MonsterVisible += (bool newVal) =>
        {
            Tween tween = GetTree().CreateTween();
            float finalVal = newVal ? 1.0f : 0.0f;
            tween.TweenProperty(_monsterSprite, "modulate:a", finalVal, 0.5);
        };
        Global.Singleton.PlayShootSound += () =>
        {
            _shootSound.Play();
        };
        RenderingServer.SetDefaultClearColor(Colors.Black);
        _attackButton.GrabFocus();

        // Connect button signals
        _attackButton.Pressed += OnAttackButtonPressed;
        _actButton.Pressed += OnActButtonPressed;
        _itemButton.Pressed += OnItemButtonPressed;
        _mercyButton.Pressed += OnMercyButtonPressed;
        _anim.AnimationFinished += OnAnimAnimationFinished;

        foreach (Godot.Button button in _buttonsContainer.GetChildren())
        {
            if (button is Godot.Button btn)
            {
                btn.PivotOffset = btn.Size / 2;
                btn.FocusEntered += () => OnFocusEntered(btn);
            }
        }
    }

    private Vector2 MonsterPosition()
    {
        Node enemyNode = GetTree().GetFirstNodeInGroup("enemy");
        if (enemyNode is Node2D node2D)
        {
            return node2D.GlobalPosition;
        }
        return Vector2.Zero;
    }

    public override async void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GonnaAttack)
        {
            _selectSound.Play();
            _textBox.Modulate = Colors.White;
            AttackBarVisibility(true);
            await _textBox.ClearText();
            _anim.Play("attack");
            GonnaAttack = false;
            IsAttacking = true;
            MonsterText = Enemy.GetMonsterText();
        }
        else if (@event.IsActionPressed("ui_accept") && IsAttacking)
        {
            IsAttacking = false;
            _anim.Pause();
            Tween tween = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quint);
            tween.TweenProperty(_attackLine, "scale", new Vector2(1.25f, 1.25f), 0.25f);
            tween.TweenProperty(_attackLine, "scale", new Vector2(1, 1), 0.25f);
            _knifeSlashSound.Play();
            _knife.Show();
            _knife.GlobalPosition = MonsterPosition();
            _knife.Play();
        }
        else if (@event.IsActionPressed("ui_accept") && GonnaAct)
        {
            _selectSound.Play();
            GonnaAct = false;
            IsChoosingAct = true;
            await _textBox.ClearText();
            _textBox.Modulate = Colors.White;
            foreach (string act in Acts)
            {
                CustomButton button = _button.Instantiate<CustomButton>();
                Label textLabel = button.GetNode<Label>("text");
                textLabel.Text = Util.Shake(act);
                button.FocusExited += () =>
                {
                    button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 0.5f);
                };
                button.Pressed += () => DoAct(act);
                _optionsContainer.AddChild(button);
            }
            if (_optionsContainer.GetChild(0) is Godot.Button firstButton)
            {
                firstButton.GrabFocus();
            }
            _uiCooldownTimer.Start();
        }
        else if (@event.IsActionPressed("ui_accept") && (IsReadingActText || IsReadingItemText))
        {
            IsReadingActText = false;
            IsReadingItemText = false;
            await _textBox.ClearText();
            MonsterSpeaking = true;
            MonsterText = Enemy.GetMonsterText();
            SpeechBubbleVisibility(true);
            _monsterTextBox.Speak(MonsterText);
        }
        else if (@event.IsActionPressed("ui_accept") && GonnaSpare)
        {
            if (CanSpare)
            {
                GonnaSpare = false;
                CanSpare = false;
                _battleDone.Play();
                _music.Stop();
                BattleWon = true;
                _monsterSprite.Modulate = new Color(_monsterSprite.Modulate.R, _monsterSprite.Modulate.G, _monsterSprite.Modulate.B, 0.5f);

                CreateTween().TweenProperty(_box, "scale", new Vector2(2, 2), 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
                CreateTween().TweenProperty(_box, "scale", new Vector2(1, 1), 0.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

                int gold = GD.RandRange(50, 75);
                int exp = 0;

                string bossName = Enemy.EnemyName;
                if (Global.BOSS_NAMES.Contains(bossName) && !Global.Singleton.IsBossKilled(bossName))
                {
                    Global.Singleton.MarkBossKilled(bossName);
                    exp = GD.RandRange(25, 50);
                    Global.Singleton.AddExp(exp);
                    Global.Singleton.playerGold += gold;
                }
                else
                {
                    Global.Singleton.playerGold += gold;
                }

                _textBox.Scroll($"Battle won\nGot {exp} EXP and {gold} Gold");
                await ToSignal(_textBox, TextBox.SignalName.FinishedScrolling);
                FadeToBlack fade2 = GetNode<FadeToBlack>("/root/Fade");
                await fade2.FadeIntoBlack();
                string returnScene = Global.Singleton.lastScenePath != "" ? Global.Singleton.lastScenePath : "uid://cnxrqinpyif6b";
                GetTree().ChangeSceneToFile(returnScene);
            }
            else if (!CanSpare)
            {
                _selectSound.Play();
            }
        }
        else if (@event.IsActionPressed("ui_accept") && MonsterSpeaking)
        {
            MonsterSpeaking = false;
            _monsterTextBox.StopTalking();
            SpeechBubbleVisibility(false);
            StartHell();
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            if (GonnaAttack)
            {
                GonnaAttack = false;
                _buttonsContainer.Show();
                _attackButton.GrabFocus();
                _textBox.Modulate = Colors.White;
                _textBox.Scroll(TurnCounter > 0 ? IdleText : EncounterText);
            }
            else if (GonnaAct)
            {
                _actButton.GrabFocus();
                _buttonsContainer.Show();
                _optionsContainer.Hide();
                _textBox.Modulate = Colors.White;
                _textBox.Scroll(TurnCounter > 0 ? IdleText : EncounterText);
                GonnaAct = false;
            }
            else if (IsChoosingAct)
            {
                foreach (Node child in _optionsContainer.GetChildren())
                {
                    child.QueueFree();
                }
                _optionsContainer.Hide();
                if (CanSpare)
                {
                    _textBox.Modulate = Colors.Yellow;
                }
                _textBox.SetNewText($"* {EnemyName}");
                IsChoosingAct = false;
                GonnaAct = true;
            }
            else if (IsChoosingItem)
            {
                foreach (Node child in _optionsContainer.GetChildren())
                {
                    child.QueueFree();
                }
                _optionsContainer.Hide();
                _buttonsContainer.Show();
                _itemButton.GrabFocus();
                _textBox.Modulate = Colors.White;
                _textBox.Scroll(TurnCounter > 0 ? IdleText : EncounterText);
                IsChoosingItem = false;
            }
            else if (GonnaSpare)
            {
                _mercyButton.GrabFocus();
                _buttonsContainer.Show();
                _optionsContainer.Hide();
                _textBox.Modulate = Colors.White;
                _textBox.Scroll(TurnCounter > 0 ? IdleText : EncounterText);
                GonnaSpare = false;
            }
        }
    }

    private async void PlayerTakeDamage(int amount, Soul soul)
    {
        PlayerHp -= amount;
        _soulHitSound.Play();

        if (PlayerHp <= 0)
        {
            _music.Stop();
            _soulBreak.Play();
            BattleLost = true;
            Node attack = GetTree().GetFirstNodeInGroup("wave");
            if (IsInstanceValid(attack))
            {
                attack.QueueFree();
            }
            PackedScene deathParticle = GD.Load<PackedScene>("uid://cvsoixker4k6d");
            Node2D particles = deathParticle.Instantiate<Node2D>();
            if (particles is CpuParticles2D cpuParticles)
            {
                cpuParticles.Color = soul.Color;
                cpuParticles.GlobalPosition = soul.GlobalPosition;
            }
            AddChild(particles);
            soul.CallDeferred("queue_free");
            await ChangeBoxSize(new Vector2(1.0f, 1.0f));
            if (particles is CpuParticles2D particles2D)
            {
                particles2D.Finished += async () =>
                {
                    _textBox.Scroll("Battle Lost...");
                    await ToSignal(_textBox, TextBox.SignalName.FinishedScrolling);
                    FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
                    await fade.FadeIntoBlack();
                    string returnScene = Global.Singleton.lastScenePath != "" ? Global.Singleton.lastScenePath : "uid://cnxrqinpyif6b";
                    GetTree().ChangeSceneToFile(returnScene);
                };
            }
        }
    }

    private void OnAttackButtonPressed()
    {
        _selectSound.Play();
        _buttonsContainer.Hide();
        if (CanSpare)
        {
            _textBox.Modulate = Colors.Yellow;
        }
        _textBox.SetNewText($"* {EnemyName}");
        GonnaAttack = true;
    }

    private async void OnAnimAnimationFinished(StringName animName)
    {
        if (animName == "attack")
        {
            IsAttacking = false;
            _attackLine.Modulate = new Color(_attackLine.Modulate.R, _attackLine.Modulate.G, _attackLine.Modulate.B, 0.0f);
            _damage.Text = "miss";
            await DamageLabelBounce();
            OnAnimAnimationFinished("monster_hurt");
        }
        else if (animName == "die")
        {
            await ChangeBoxSize(new Vector2(1.0f, 1.0f));
            _textBox.Modulate = Colors.Red;
            int exp = GD.RandRange(25, 50);
            int gold = GD.RandRange(20, 30);

            string bossName = Enemy.EnemyName;
            if (Global.BOSS_NAMES.Contains(bossName) && !Global.Singleton.IsBossKilled(bossName))
            {
                Global.Singleton.MarkBossKilled(bossName);
                Global.Singleton.AddExp(exp);
                Global.Singleton.playerGold += gold;
            }
            else
            {
                Global.Singleton.playerGold += gold;
                exp = 0;
            }

            _textBox.Scroll($"Battle won\nGot {exp} EXP and {gold} Gold");
            await ToSignal(_textBox, TextBox.SignalName.FinishedScrolling);
            FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
            await fade.FadeIntoBlack();
            string returnScene = Global.Singleton.lastScenePath != "" ? Global.Singleton.lastScenePath : "uid://cnxrqinpyif6b";
            GetTree().ChangeSceneToFile(returnScene);
        }
        else if (animName == "monster_hurt")
        {
            AttackBarVisibility(false);
            if (EnemyHp <= 0)
            {
                BattleWon = true;
                _anim.Play("die");
                _music.Stop();
                _battleDone.Play();
                return;
            }
            MonsterSpeaking = true;
            SpeechBubbleVisibility(true);
            _monsterTextBox.Speak(MonsterText);
        }
    }

    private int _waveIndex = 0;
    private async void StartHell()
    {
        _buttonsContainer.Hide();
        Node2D wave = BulletWaves[_waveIndex % BulletWaves.Count].Instantiate<Node2D>();
        _waveIndex += 1;
        if (wave is Wave waveInstance)
        {
            Soul soul = Soul.NewSoul(waveInstance.Mode);
            soul.GlobalPosition = _attackBar.GlobalPosition;
            soul.TookDamage += PlayerTakeDamage;
            await ChangeBoxSize(waveInstance.BoxSize, waveInstance.BoxSizeChangeTime);
            AddChild(soul);
            AddChild(wave);
        }
    }

    private async void FinishHell(Node2D wave, Soul soul)
    {
        wave.QueueFree();
        if (BattleWon || BattleLost) return;
        TurnCounter += 1;
        _buttonsContainer.Show();
        soul.QueueFree();
        if (wave is Wave waveInstance)
        {
            await ChangeBoxSize(new Vector2(1.0f, 1.0f), waveInstance.BoxSizeChangeTime);
        }
        IdleText = Enemy.GetIdleText();
        _textBox.Scroll(IdleText);
        _attackButton.GrabFocus();
    }

    private void OnActButtonPressed()
    {
        _selectSound.Play();
        _buttonsContainer.Hide();
        _optionsContainer.Show();
        if (CanSpare)
        {
            _textBox.Modulate = Colors.Yellow;
        }
        _textBox.SetNewText($"* {EnemyName}");
        GonnaAct = true;
    }

    private void DoAct(string actName)
    {
        if (_uiCooldownTimer.TimeLeft > 0) return;
        _selectSound.Play();
        foreach (Node child in _optionsContainer.GetChildren())
        {
            child.QueueFree();
        }
        _optionsContainer.Hide();
        _textBox.Scroll(Enemy.DoActGetText(actName));
        IsChoosingAct = false;
        IsReadingActText = true;
    }

    private void UseItem(Item item)
    {
        if (_uiCooldownTimer.TimeLeft > 0) return;
        _useItemSound.Play();
        foreach (Node child in _optionsContainer.GetChildren())
        {
            child.QueueFree();
        }
        _optionsContainer.Hide();
        PlayerHp += item.Amount;
        _textBox.Scroll(item.Text);
        Items.Remove(item);
        for (int i = 0; i < Global.Singleton.battleInventory.Count; i++)
        {
            if (Global.Singleton.battleInventory[i].ItemName == item.ItemName)
            {
                Global.Singleton.battleInventory.RemoveAt(i);
                break;
            }
        }
        IsChoosingItem = false;
        IsReadingItemText = true;
    }

    private void OnMercyButtonPressed()
    {
        _selectSound.Play();
        GonnaSpare = true;
        _buttonsContainer.Hide();
        if (CanSpare)
        {
            _textBox.Modulate = Colors.Yellow;
        }
        _textBox.SetNewText($"* {EnemyName}");
    }

    private void OnItemButtonPressed()
    {
        _selectSound.Play();
        if (Items.Count <= 0) return;
        IsChoosingItem = true;
        _buttonsContainer.Hide();
        _optionsContainer.Show();
        _textBox.ClearText();
        foreach (Item item in Items)
        {
            CustomButton button = _button.Instantiate<CustomButton>();
            Label textLabel = button.GetNode<Label>("text");
            textLabel.Text = Util.Shake(item.ItemName);
            button.FocusExited += () =>
            {
                button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 0.5f);
            };
            button.Pressed += () => UseItem(item);
            _optionsContainer.AddChild(button);
        }
        if (_optionsContainer.GetChild(0) is Godot.Button firstButton)
        {
            firstButton.GrabFocus();
        }
        _uiCooldownTimer.Start();
    }

    private void OnKnifeAnimationFinished()
    {
        _knife.Hide();
        _monsterHurtSound.Play();
        int distanceFromCentre = Mathf.RoundToInt(Mathf.Abs(_attackLine.GlobalPosition.X - _attackBar.GlobalPosition.X));
        int damage = Mathf.RoundToInt((575 - distanceFromCentre) / 10.0f);
        _damage.Text = damage.ToString();
        DamageLabelBounce();
        EnemyHp -= damage;
        _anim.Play("monster_hurt");
    }

    private void MonsterSpeakingAnim()
    {
        Tween tween = GetTree().CreateTween();
        Vector2 ogDim = _monsterSprite.Scale;
        Vector2 animDim = ogDim + new Vector2(ogDim.X * 0.1f, ogDim.Y * -0.05f);
        float delta = 0.2f;
        for (int i = 0; i < 2; i++)
        {
            tween.TweenProperty(_monsterSprite, "scale", animDim, delta);
            tween.TweenProperty(_monsterSprite, "scale", ogDim, delta);
        }
    }

    private async Task ChangeBoxSize(Vector2 newSize, float delta = 0.3f)
    {
        Tween tween = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(_box, "scale:x", newSize.X, delta);
        tween.TweenProperty(_box, "scale:y", newSize.Y, delta);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_box == null) return;
        Rect2 boxRect = new Rect2(_box.GlobalPosition, _box.Size * _box.Scale);
        DrawRect(boxRect, Colors.White, false, 10);
    }

    private void AttackBarVisibility(bool visible)
    {
        float newVal = visible ? 1.0f : 0.0f;
        float delta = visible ? 0.05f : 0.2f;
        Tween tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Bounce).SetParallel(true);
        tween.TweenProperty(_attackLine, "modulate:a", newVal, delta);
        tween.TweenProperty(_attackBar, "modulate:a", newVal, delta);
    }

    private void SpeechBubbleVisibility(bool visible)
    {
        float newVal = visible ? 1.0f : 0.0f;
        Tween tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Bounce).SetParallel(true);
        tween.TweenProperty(_speechBox, "modulate:a", newVal, 0.25f);
    }

    private void OnFocusEntered(Godot.Button button)
    {
        button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 1.0f);
        _moveSound.Play();
        Tween tween = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Bounce);
        tween.TweenProperty(button, "scale", new Vector2(1.5f, 1.5f), 0.2f);
        tween.TweenProperty(button, "scale", new Vector2(1, 1), 0.1f);
    }

    private async Task DamageLabelBounce()
    {
        Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(_damage, "modulate:a", 1.0f, 0.1f);
        tween.TweenProperty(_damage, "scale", new Vector2(1.5f, 1.5f), 0.2f);
        tween.TweenProperty(_damage, "scale", new Vector2(1, 1), 0.1f);
        tween.TweenProperty(_damage, "modulate:a", 0.0f, 0.1f);
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
