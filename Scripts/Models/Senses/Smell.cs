using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TheValley.Scripts.Models.Metabolism;
using TheValley.Scripts.Models.Item.Consumable;


namespace TheValley.Scripts.Models.Senses
{
    public partial class Smell : Node3D
    {
        [Signal] public delegate void ObjectSmeltEventHandler(Node3D obj);
        private Area3D SmellArea {get;set;}
        private Vector3 SmellAreaScale {get;set;}
        private Creature Creature {get;set;}

        public Smell(Creature creature, Vector3 smellAreaScale) {
            SmellAreaScale = smellAreaScale;
            Creature = creature;
            InitializeSmell();
            ConnectSmellAreaSignals();
        }
        private void InitializeSmell()
        {
            /**
            ** Default values for testing, but it will be needed to be dynamic later, to handle
            ** new creatures setup easily
            **/
            // Check if SmellArea already exists
            if (Creature.GetNodeOrNull<Area3D>("SmellArea") == null)
            {
                SmellArea = new Area3D
                {
                    Name = "SmellArea",
                    Scale = SmellAreaScale,
                    CollisionLayer = 2,
                    CollisionMask = 2,
                    Monitoring = true
                };
                // Create and configure the CollisionShape3D
                var collisionShape = new CollisionShape3D();
                var shape = new SphereShape3D {
                    Radius = SmellArea.Scale.X / 2
                };
                collisionShape.Shape = shape;

                SmellArea.AddChild(collisionShape);
                Creature.AddChild(SmellArea);
            }
            else
            {
                SmellArea = Creature.GetNode<Area3D>("SmellArea");
            }
        }

        // Method to connect signals for the smell area
        private void ConnectSmellAreaSignals()
        {
            if (SmellArea != null)
            {
                SmellArea.BodyEntered += OnSmellAreaBodyEntered;
                SmellArea.BodyExited += OnSmellAreaBodyExited;
            }
        }

        // Method to handle when a body enters the smell area
        private void OnSmellAreaBodyEntered(Node3D body)
        {
            if (body is Food || body is Water)
            {
                GD.Print("Smell detected and added to list: " + body.Name);
                EmitSignal(nameof(ObjectSmelt), body); // Emit custom signal
            }
        }

        // Method to handle when a body exits the smell area
        private static void OnSmellAreaBodyExited(Node3D body)
        {
            GD.Print("Smell lost : " + body.Name);
        }

        // Method to get the list of currently detected smelt items (food or water)
        public List<Node3D> GetSmeltItems()
        {
            return SmellArea.GetOverlappingBodies().ToList();
        }
    }
}