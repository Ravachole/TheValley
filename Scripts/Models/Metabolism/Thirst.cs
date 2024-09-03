namespace TheValley.Scripts.Models.Metabolism
{
    public class Thirst : Metabolism 
    {
        public Thirst() : base(1f, 0.0f) {}
        public override void Update(double delta)
        {
            // Apply thirst-specific update logic here
            DrainOverTime(delta);
        }
    }
}