using Godot;
using TheValley.Scripts.Models.Item;

namespace TheValley.Scripts.Models.Metabolism
{
    public class MemoryEntry
    {
        public GeneralItem item {get; set;}
        public float TimeStamp { get; set; }  // Time when it was last seen

        public MemoryEntry(GeneralItem _item, float timeStamp)
        {
            item = _item;
            TimeStamp = timeStamp;
        }
    }
}