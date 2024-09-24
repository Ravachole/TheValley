using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TheValley.Scripts.Models.Metabolism;
using TheValley.Scripts.Models.Item.Consumable;

namespace TheValley.Scripts.Models.Senses
{
    public partial class Vision : Node3D
    {
        [Signal] public delegate void ObjectSeenEventHandler(Node3D obj);
        private Area3D _visionArea;
        private float _visionRange = 150.0f;  // Radius of the vision sphere
        private float _fovAngle = 90.0f;     // Field of view in degrees
        private List<Node3D> _visibleObjects = new List<Node3D>();
        public override void _Ready()
        {
            base._Ready();
            Initialize();
            ConnectVisionSignals();
        }

        public void Initialize()
        {
            // Initialize the vision area
            _visionArea = new Area3D();
            _visionArea.Monitoring = true;
            _visionArea.SetCollisionMaskValue(1, true);
            _visionArea.SetCollisionMaskValue(2, true);
            

            // Create and configure the CollisionShape3D with a SphereShape3D
            var collisionShape = new CollisionShape3D();
            var sphereShape = new SphereShape3D
            {
                Radius = _visionRange
            };
            collisionShape.Shape = sphereShape;

            _visionArea.AddChild(collisionShape);
            AddChild(_visionArea);
        }

        private void ConnectVisionSignals()
        {
            if (_visionArea != null)
            {
                _visionArea.BodyEntered += OnVisionAreaBodyEntered;
                _visionArea.BodyExited += OnVisionAreaBodyExited;
            }
        }

        private void OnVisionAreaBodyEntered(Node3D body)
        {
            if (IsWithinVisionArc(body))
            {
                // Check if the object is food or water and emit the custom signal
                if (body.IsInGroup("food") || body.IsInGroup("water"))
                {
                    EmitSignal(nameof(ObjectSeen), body); // Emit custom signal
                }
            }
        }

        private void OnVisionAreaBodyExited(Node3D body)
        {
            if (_visibleObjects.Exists(node => node.Name == body.Name))
            {
                _visibleObjects.Remove(body);
            }
        }
        // Define a vision arc from the sphere colliderSphere3D
        private bool IsWithinVisionArc(Node3D body)
        {
            Vector3 toBody = (body.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
            Vector3 forward = GlobalTransform.Basis.Z.Normalized();
            float angleToBody = Mathf.RadToDeg(Mathf.Acos(forward.Dot(toBody)));

            // Check if the object is within the field of view (FOV)
            if (angleToBody > (_fovAngle / 2))
            {
                return false;
            }

            // Create a raycast to check for obstacles
            var spaceState = GetWorld3D().DirectSpaceState;
            var raycastParams = new PhysicsRayQueryParameters3D
            {
                From = GlobalTransform.Origin,
                To = body.GlobalTransform.Origin,
                CollisionMask = 1, // Collision layer for obstacles
            };

            var result = spaceState.IntersectRay(raycastParams);
            
            // If there's a hit and it's not the target resource, return false
            if (result.Count > 0)
            {
                return false;
            }

            return true;
        }

        public List<Node3D> GetVisibleObjects()
        {
            return _visibleObjects;
        }
    }

}
