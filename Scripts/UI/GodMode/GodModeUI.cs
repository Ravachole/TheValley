using Godot;
using System.Collections.Generic;
using TheValley.Scripts.Models;

public partial class GodModeUI : Control
{
    private GridContainer entityGrid;
    private List<Node3D> creatures;
    private Camera3D currentCamera;
    private Camera3D godModeCamera;


    public override void _Ready()
    {
        entityGrid = GetNode<GridContainer>("EntityGridContainer");
        currentCamera = godModeCamera = GetNode<Camera3D>("../MJCamera");
        creatures = new List<Node3D>();

        FindAllCreatures(GetTree().Root);
        UpdateEntityList();

        // Set up a timer to refresh the stats periodically
        Timer updateTimer = new Timer();
        updateTimer.WaitTime = 1.0f; // Update every second
        updateTimer.Autostart = true;
        updateTimer.OneShot = false;
        updateTimer.Timeout += () => UpdateEntityList(); // Refresh UI
        AddChild(updateTimer);
    }

    private void UpdateEntityList()
    {
        entityGrid.QueueFreeChildren();

        foreach (Creature creature in creatures)
        {
            HBoxContainer row = new HBoxContainer();
            Label nameLabel = new Label { Text = $"{creature.Name}" };
            row.AddChild(nameLabel);

            Label hungerLabel = new Label { Text = $"Hunger: {Mathf.Round(creature.Hunger.Current)}" };
            Label thirstLabel = new Label { Text = $"Thirst: {Mathf.Round(creature.Thirst.Current)}" };
            Label staminaLabel = new Label { Text = $"Stamina: {Mathf.Round(creature.Stamina.Current)}" };
            Label StatusLabel = new Label { Text = $"Status: {creature.CurrentState}" };
            
            row.AddChild(hungerLabel);
            row.AddChild(thirstLabel);
            row.AddChild(staminaLabel);
            row.AddChild(StatusLabel);

            Button switchToCameraButton = new Button { Text = "Follow Camera" };
            switchToCameraButton.Pressed += () => OnSwitchCameraPressed(creature);
            row.AddChild(switchToCameraButton);
            Button backToGodModeCamera = new Button { Text = "X" };
            backToGodModeCamera.Pressed += () => OnSwitchToGodModeCamera(creature);
            row.AddChild(backToGodModeCamera);

            entityGrid.AddChild(row);
        }
    }

    public void OnSwitchToGodModeCamera(Creature creature)
    {
        currentCamera = godModeCamera;
        currentCamera.Current = true;
    }
    public void OnSwitchCameraPressed(Creature creature)
    {
        Camera3D creatureCamera = creature.GetNode<Camera3D>("Camera3D");

        if (currentCamera != null)
        {
            currentCamera.Current = false;
        }

        creatureCamera.Current = true;
        currentCamera = creatureCamera;

        GD.Print($"Switched to camera of {creature.Name}");
    }

    void FindAllCreatures(Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is Creature creature)
            {
                creatures.Add(creature);
            }
            FindAllCreatures(child);
        }
    }
}

public static class NodeExtensions
{
    public static void QueueFreeChildren(this Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            child.QueueFree();
        }
    }
}
