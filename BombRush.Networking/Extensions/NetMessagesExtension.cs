using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace BombRush.Networking.Extensions
{
    public static class NetMessagesExtension
    {
        public static void WritePackedVector(this NetOutgoingMessage msg, Vector2 vector)
        {
            msg.Write(new HalfSingle(vector.X).PackedValue);
            msg.Write(new HalfSingle(vector.Y).PackedValue);
        }

        public static Vector2 ReadPackedVector2(this NetIncomingMessage msg)
        {
            return new Vector2(
                (new HalfSingle { PackedValue = msg.ReadUInt16() }).ToSingle(),
                (new HalfSingle { PackedValue = msg.ReadUInt16() }).ToSingle());
        }
    }
}
