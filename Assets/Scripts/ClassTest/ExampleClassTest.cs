
using UnityEngine;

namespace Example
{
    public abstract class Vehicle : MonoBehaviour   // Is a vehicle
    {
        protected Color color = Color.black;    // Has a color
        protected Engine engine;                // Has a engine
        public abstract void Drive();

    }

    public class Engine
    {
        public int horsePower;
    }
    public class Car : Vehicle  // Is a vehicle
    {
        public override void Drive()
        {
            Debug.Log(message: "Driving a car");
        }
    }

    public class Truck : Vehicle    // Is a vehicle
    {
        public override void Drive()
        {
            Debug.Log(message: "Driving a truck");
        }
    }

    public class BlackCar : Car
    {
        public override void Drive()
        {
            Debug.Log(message: "Deriving a black car");
        }
    }
}

namespace Example1
{
    public enum ResourceType { Empty, Stone, Oak, Birch, Pine, Iron, Coal, Gold, Diamond}

    public abstract class BaseNode : MonoBehaviour
    {
        public abstract ResourceType Gather();
    }

    public abstract class SpecialNode : BaseNode
    {
        public override ResourceType Gather()
        {
            return ResourceType.Empty;
        }
        public abstract ResourceType SpecialGather();
    }

    public class IronNode : SpecialNode
    {
        public override ResourceType Gather()
        {
            return ResourceType.Stone;
        }
        public override ResourceType SpecialGather()
        {
            return ResourceType.Iron;
        }
    }

    public class StoneNode : BaseNode
    {
        public override ResourceType Gather()
        {
            return ResourceType.Stone;
        }
    }

    public class OakNode : BaseNode
    {
        public override ResourceType Gather()
        {
            return ResourceType.Oak;
        }

    }

}

