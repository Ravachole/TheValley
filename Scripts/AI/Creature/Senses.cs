
using Godot;
public partial class Senses : Area3D {
    [Export] public Area3D selfCollider;
    [Export] public Area3D[] currentResources;
    [Export] public Area3D[] currentPredators;
    [Export] public bool checkLos = false;
    public RayCast3D rayCast3D;

    public override void _Ready()
    {
        Area3D area = GetNode("AreaLake/Lake") as Area3D;
        // area.Connect("OnAreaEntered", new Callable(this, "OnAreaEntered"));
        area.AreaEntered += OnAreaEntered;
        area.AreaExited += OnAreaExited;
        if (checkLos) {
            rayCast3D = GetNode("DirectVisionRayCast") as RayCast3D;
        }
    }
    // If enter a zone, ignore his self collider then add the zone in his "radar"
    public void OnAreaEntered(Area3D currentArea) {
        GD.Print("OnAreaEntered");
        if (currentArea != selfCollider) {
            currentResources[currentResources.Length+1] = currentArea;
        }
    }
    // Remove the zone when exiting it TODO : implement same with predators, others creatures
    public void OnAreaExited(Area3D currentArea) {
        GD.Print("OnAreaExited");
        for (int i = 0; i < currentResources.Length; i++) {
            if (currentResources[i] != null && currentResources[i] == currentArea) {
                currentResources[i].RemoveChild(currentArea);
            }
        }
    }

    // Check resources visibility
    public Node3D getAreas() {
        GD.Print("getRes");
        if (currentResources != null && currentResources.Length > 0) {
            if (checkLos) {
                if (HasLos(currentResources[0].GlobalPosition)) {
                    return currentResources[0] as Node3D;
                } 
                else {
                    return null;
                }
            } 
            else {
                return currentResources[0] as Node3D;
            }
        } 
        else {
            return null;
        }
    }
    // Check if AI is seing smthg
    public  bool HasLos(Vector3 target) {
        rayCast3D.Enabled = true;
        rayCast3D.LookAtFromPosition(rayCast3D.GlobalPosition, target, new Vector3(0, 1, 0));
        rayCast3D.TargetPosition = -target;
        rayCast3D.ForceRaycastUpdate();

        if (rayCast3D.IsColliding()) {
            rayCast3D.Enabled = false;
            return true;
        }
        else {
            rayCast3D.Enabled = false;
            return false;
        }
    }
}