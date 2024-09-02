using Godot;
using TheValley.Scripts.AI.Behavior;

namespace TheValley.Scripts.Models
{
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
            Thirst.Update(delta);
            Hunger.Update(delta);
            Stamina.Update(delta);
            // delta updated
            Delta = (float)delta;
            //Update behavior tree
            _behaviorTree.Update(this);
            // Apply the behavior tree velocity with state check
            if (CurrentState != CreatureState.Wandering)
            {
                Velocity = Vector3.Zero;
            }
            MoveAndSlide();
        }
    }
}
