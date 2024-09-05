using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheValley.Scripts.Models.Senses
{
    public partial class Vision : Node3D
    {
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
                _visibleObjects.Add(body);
                GD.Print("Object in vision: " + body.Name);
            }
        }

        private void OnVisionAreaBodyExited(Node3D body)
        {
            if (_visibleObjects.Exists(node => node.Name == body.Name))
            {
                _visibleObjects.Remove(body);
                GD.Print("Object left vision: " + body.Name);
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
            // Perform a raycast to ensure there are no obstacles in the way
            var spaceState = GetWorld3D().DirectSpaceState;
            var parent = GetParent() as Node3D;
            var query = PhysicsRayQueryParameters3D.Create(GlobalTransform.Origin, body.GlobalTransform.Origin, 1);
            
            var result = spaceState.IntersectRay(query);

            // If the ray hits something, it means there's an obstacle in the way
            if (result.Count > 0)
            {
                GD.Print("Object blocked by an obstacle: " + result["collider"].ToString());
                return false;
            }

            // If no obstacle was hit, the object is within the vision arc and in line of sight
            return true;
        }

        public List<Node3D> GetVisibleObjects()
        {
            return _visibleObjects;
        }
    }

}
