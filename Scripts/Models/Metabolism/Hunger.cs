namespace TheValley.Scripts.Models.Metabolism
{
    public class Hunger : Metabolism 
    {

        public Hunger() : base(0.6f, 0.0f) {}
        public override void Update(double delta)
        {
            // Apply hunger-specific update logic here
            DrainOverTime(delta);
        }
    }
}