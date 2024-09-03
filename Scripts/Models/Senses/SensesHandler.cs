using System;
using Godot;
using System.Linq;
using System.Collections.Generic;

namespace TheValley.Scripts.Models.Senses
{
    public class SensesHandler
    {
        private Area3D _smellArea {get;set;}
        private Vector3 _smellAreaScale {get;set;}
        private Creature _creature {get;set;}
        private RayCast3D _visionCast {get;set;}

        public SensesHandler(Creature creature, Vector3 smellAreaScale) {
            _smellAreaScale = smellAreaScale;
            _creature = creature;
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
            if (_creature.GetNodeOrNull<Area3D>("SmellArea") == null)
            {
                _smellArea = new Area3D
                {
                    Name = "SmellArea",
                    Scale = _smellAreaScale,
                    CollisionLayer = 2,
                    CollisionMask = 1 | 2,
                    Monitoring = true
                };
                // Create and configure the CollisionShape3D
                var collisionShape = new CollisionShape3D();
                var shape = new SphereShape3D {
                    Radius = _smellArea.Scale.X / 2
                };
                collisionShape.Shape = shape;

                _smellArea.AddChild(collisionShape);
                _creature.AddChild(_smellArea);
            }
            else
            {
                _smellArea = _creature.GetNode<Area3D>("SmellArea");
            }
        }

        // Method to connect signals for the smell area
        private void ConnectSmellAreaSignals()
        {
            if (_smellArea != null)
            {
                _smellArea.BodyEntered += OnSmellAreaBodyEntered;
                _smellArea.BodyExited += OnSmellAreaBodyExited;
            }
        }

        // Method to handle when a body enters the smell area
        private static void OnSmellAreaBodyEntered(Node3D body)
        {
            if (body.IsInGroup("food") || body.IsInGroup("water"))
            {
                GD.Print("Smell detected and added to list: " + body.Name);
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
            return _smellArea.GetOverlappingBodies().ToList();
        }
    }
}