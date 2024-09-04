using Godot;

namespace TheValley.Scripts.Models.Item.Consumable
{
    public partial class Water : Item 
    {
        public Water() : base("water") {}

        public override void Update()
        {
            base.Update(); // Call the base Update method to handle state changes

            GD.Print($"[Water] {Name} current value is: {Value}, its state is : {State}");
        }
    }
}