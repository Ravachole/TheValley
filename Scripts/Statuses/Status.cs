abstract class Status
{
    private float minimum = 0;
    private float maximum = 0;
    private float current = 100;
}

class Thirst : Status {}
class Hunger : Status {}
class Energy : Status {}
class Stress : Status {}

