using GTA.Math;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal struct EntityPosition
    {
        internal Vector3 Position;
        internal float Heading;

        internal EntityPosition(Vector3 pos, float heading)
        {
            Position = pos;
            Heading = heading;
        }
    }
}