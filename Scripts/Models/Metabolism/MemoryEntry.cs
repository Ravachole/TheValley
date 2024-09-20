using Godot;

namespace TheValley.Scripts.Models.Metabolism
{
    public class MemoryEntry
    {
        public Vector3 Position { get; set; }
        public string ResourceType { get; set; }  // e.g., "food" or "water"
        public float TimeStamp { get; set; }  // Time when it was last seen

        public MemoryEntry(Vector3 position, string resourceType, float timeStamp)
        {
            Position = position;
            ResourceType = resourceType;
            TimeStamp = timeStamp;
        }
    }
}