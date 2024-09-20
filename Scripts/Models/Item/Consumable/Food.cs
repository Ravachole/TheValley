using Godot;

namespace TheValley.Scripts.Models.Item.Consumable
{
    public partial class Food : GeneralItem 
    {
        public Food() : base("food",100.0f,0.5f) {}

        public override void Update()
        {
            base.Update(); // Call the base Update method to handle state changes

            GD.Print($"[Food] {Name} current value is: {Value}, its state is : {State}");
        }
    }
}