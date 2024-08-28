using Godot;

public partial class Herbivore : Creature {

    private HerbivoreBehaviorTree _behaviorTree;
    public override void _Ready()
    {
        _behaviorTree = new HerbivoreBehaviorTree();
        _waterDetectionArea = GetNode<Area3D>("WaterDetectionArea");
        _foodDetectionArea = GetNode<Area3D>("FoodDetectionArea");

        _waterDetectionArea.BodyEntered += OnWaterEntered;
        _foodDetectionArea.BodyEntered += OnFoodEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        //Update behavior tree
        _behaviorTree.Update(this);
        // Apply the behavior tree velocity
        Velocity = _velocity;
        MoveAndSlide();
    }
}
