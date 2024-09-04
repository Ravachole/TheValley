using Godot;
using TheValley.Scripts.AI.Behavior;
using TheValley.Scripts.Models.Senses;

namespace TheValley.Scripts.Models
{
    public partial class Herbivore : Creature {

        private HerbivoreBehaviorTree _behaviorTree;

        public override void _Ready()
        {
            _behaviorTree = new HerbivoreBehaviorTree();
            Senses = new SensesHandler(this, new Vector3(25,25,25));
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
