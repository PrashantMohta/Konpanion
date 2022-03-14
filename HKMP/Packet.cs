
using Hkmp;
using Hkmp.Networking.Packet;

namespace Konpanion{
    public enum PacketId {
        KonpanionClientUpdate, // sent by client 
        KonpanionServerUpdate, // sent by server
    }

    public class KonpanionClientUpdateData : IPacketData {
        // Denote whether this data should be handled as reliable
        public bool IsReliable => false;
        // Whether to drop data if a newer version of the data is also included in the packet
        public bool DropReliableDataIfNewerExists => true;

        // The data we are transmitting
        public Hkmp.Math.Vector2 pos { get; set; }
        public State anim { get; set; }
        public Direction dir { get; set; }

        public void WriteData(IPacket packet) {
            //order of read should be same as order of write
            packet.Write(pos);
            packet.Write((int)anim);
            packet.Write((int)dir);
        }

        public void ReadData(IPacket packet) {
            //order of read should be same as order of write
            pos = packet.ReadVector2();
            anim = (State)packet.ReadInt();
            dir = (Direction)packet.ReadInt();
        }
    }

    public class KonpanionServerUpdateData : IPacketData {
        // Denote whether this data should be handled as reliable
        public bool IsReliable => false;
        // Whether to drop data if a newer version of the data is also included in the packet
        public bool DropReliableDataIfNewerExists => false;

        // The data we are transmitting
        public ushort playerId {get; set;}
        public Hkmp.Math.Vector2 pos { get; set; }
        public State anim { get; set; }
        public Direction dir { get; set; }

        public void WriteData(IPacket packet) {
            //order of read should be same as order of write
            packet.Write(playerId);
            packet.Write(pos);
            packet.Write((int)anim);
            packet.Write((int)dir);
        }

        public void ReadData(IPacket packet) {
            //order of read should be same as order of write
            playerId = packet.ReadUShort();
            pos = packet.ReadVector2();
            anim = (State)packet.ReadInt();
            dir = (Direction)packet.ReadInt();
        }
    }

}