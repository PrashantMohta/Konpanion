using Hkmp.Api.Server;
using Hkmp.Networking.Packet;
using UnityEngine;
using System.Linq;

namespace Konpanion {
    public class KonpanionServer : ServerAddon {
        internal IServerApi serverApi;
        public KonpanionServer(IServerApi serverApi) : base(serverApi) {
            this.serverApi = serverApi;
        }

        public override void Initialize() {
            Modding.Logger.Log("Initializing server-side KonpanionServer addon!");
            //serverApi.ServerManager
            var netReceiver = serverApi.NetServer.GetNetworkReceiver<PacketId>(this,InstantiatePacket);
            var netSender = serverApi.NetServer.GetNetworkSender<PacketId>(this);

            netReceiver.RegisterPacketHandler<KonpanionClientUpdateData>(
                PacketId.KonpanionClientUpdate,
                async (id, packetData) => {
                    // Get the float from the packet data
                    var pos = packetData.pos;
                    var anim = packetData.anim;
                    var dir = packetData.dir;

                    // Log the player ID and the float value
                    Modding.Logger.Log($"Received on server from {id}: {pos}, {anim}, {dir}");
                    
                    //Then send response data to all other clients 
                    var players = serverApi.ServerManager.Players;
                    for(var i = 0; i < players.Count ; i++){
                        var player = players.ElementAt(i);
                        if(player.Id != id){
                            netSender.SendSingleData(PacketId.KonpanionServerUpdate, new KonpanionServerUpdateData {
                                playerId = id,
                                pos = packetData.pos,
                                anim = packetData.anim,
                                dir = packetData.dir
                            }, player.Id);
                        }
                    }
                }
            );
        }

        private static IPacketData InstantiatePacket(PacketId packetId) {
            switch (packetId) {
                case PacketId.KonpanionClientUpdate:
                    return new KonpanionClientUpdateData();
            }
            return null;
        }

        protected override string Name => "Konpanion";
        protected override string Version => "0.0.1";
        public override bool NeedsNetwork => true;
    }
}
