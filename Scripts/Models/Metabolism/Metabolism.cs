using Godot;
abstract class Metabolism
{
    private float minimum = 0;
    private float maximum = 0;
    private float current = 100;
    private float drain = 0;
    private float regeneration = 0;
}

class Thirst : Metabolism {}
class Hunger : Metabolism {}
class Energy : Metabolism {}
class Stress : Metabolism {}
class Health : Metabolism {}
class Stamina : Metabolism {}
class Growth : Metabolism {}

