using Godot;

public partial class BattleZone : Area2D
{
    [Export]
    public PackedScene EnemyScene { get; set; }
    
    [Export]
    public string BossName { get; set; } = "";

    public override void _Ready()
    {
        // Check if this boss is already killed and hide the zone if so
        if (BossName != "" && Global.Singleton.IsBossKilled(BossName))
        {
            Visible = false;
            CallDeferred("set", "monitoring", false);
            CallDeferred("set", "monitorable", false);
            return;
        }
        
        BodyEntered += _OnBodyEntered;
    }
    
    private void _OnBodyEntered(Node2D body)
    {
        if (body.Name == "Jugador" || body.Name == "Player")
        {
            StartBattle();
        }
    }

    private async void StartBattle()
    {
        if (EnemyScene == null)
        {
            // Default to cherry if none specified
            EnemyScene = GD.Load<PackedScene>("res://enemy_data/cherry.tscn");
        }
        
        // Save current scene and player position before battle
        string currentScene = GetTree().CurrentScene.SceneFilePath;
        Global.Singleton.lastScenePath = currentScene;
        
        // Find player and save position
        Node2D player = null;
        Node sceneRoot = GetTree().CurrentScene;
        
        // Try different ways to find the player
        Node playerNode = sceneRoot.GetNodeOrNull("Player");
        if (playerNode != null && playerNode is Node2D)
        {
            player = (Node2D)playerNode;
        }
        else
        {
            playerNode = sceneRoot.GetNodeOrNull("Jugador");
            if (playerNode != null && playerNode is Node2D)
            {
                player = (Node2D)playerNode;
            }
            else
            {
                playerNode = sceneRoot.GetNodeOrNull("Objetos/Jugador");
                if (playerNode != null && playerNode is Node2D)
                {
                    player = (Node2D)playerNode;
                }
            }
        }
        
        if (player == null)
        {
            // Try finding by groups
            Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("player");
            foreach (Node p in players)
            {
                if (p is Node2D)
                {
                    player = (Node2D)p;
                    break;
                }
            }
        }
        
        if (player != null && player is Node2D)
        {
            Global.Singleton.lastPlayerPosition = player.GlobalPosition;
        }
        else
        {
            Global.Singleton.lastPlayerPosition = Vector2.Zero;
        }
        
        Enemy enemy = EnemyScene.Instantiate<Enemy>();
        if (enemy != null)
        {
            // Ensure the enemy is properly initialized by adding it to the scene tree temporarily
            // This ensures all exported properties are loaded from the scene file
            // The properties are set when the node enters the scene tree
            AddChild(enemy);
            // Wait for the node to be fully initialized (properties loaded, _Ready called)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            RemoveChild(enemy);
            
            Battle.Enemy = enemy;
            FadeToBlack fade = GetNode<FadeToBlack>("/root/Fade");
            await fade.FadeIntoBlack();
            GetTree().ChangeSceneToFile("uid://45qmet5s5aix");
        }
    }
}
