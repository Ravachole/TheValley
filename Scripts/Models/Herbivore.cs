using System.Linq;
using Godot;
using TheValley.Scripts.AI.Behavior;
using TheValley.Scripts.Models.Senses;

namespace TheValley.Scripts.Models
{
    public partial class Herbivore : Creature {

        private HerbivoreBehaviorTree BehaviorTree;

        public override void _Ready()
        {
            BehaviorTree = new HerbivoreBehaviorTree();
            NavigationAgent = GetNode("HerbivorePathFinder") as NavigationAgent3D;
            Smell = new Smell(this, new Vector3(25,25,25));
            Vision = new Vision();
            AddChild(Vision);
            // Defer the signal connections to ensure Vision and Smell are ready
            CallDeferred(nameof(ConnectMemorySignals));
        }

        private void OnObjectDetected(Node3D obj)
        {
            // Logic to add the object to memory
            AddToMemory(obj.Position, obj.GetGroups().FirstOrDefault(),Time.GetTicksUsec());
            GD.Print($"{obj.Name} added to memory as a resource.");
        }

        private void ConnectMemorySignals()
        {
            // Now connect the signals
            Vision.Connect(nameof(Vision.ObjectSeen), new Callable(this, nameof(OnObjectDetected)));
            Smell.Connect(nameof(Smell.ObjectSmelt), new Callable(this, nameof(OnObjectDetected)));
        }

        public override void _PhysicsProcess(double delta)
        {
            Thirst.Update(delta);
            Hunger.Update(delta);
            Stamina.Update(delta);
            // delta updated
            Delta = (float)delta;
            //Update behavior tree
            BehaviorTree.Update(this);
            // Apply the behavior tree velocity with state check
            if (CurrentState != CreatureState.Wandering)
            {
                Velocity = Vector3.Zero;
            }
            MoveAndSlide();
        }
    }
}
