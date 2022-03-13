using Hkmp.Api.Client;
using Hkmp.Networking.Packet;
using UnityEngine;

namespace Konpanion {
    public class KonpanionClient : ClientAddon {

        // The client addon instance
        internal static ClientAddon clientAddon;
        // The client API
        internal static IClientApi clientApi;


        public KonpanionClient(IClientApi _clientApi) : base(clientApi) {
            clientApi = _clientApi;
            clientAddon = this;
        }

        internal static void sendUpdate(Vector2 position,State state,Direction dir){
            var netSender = clientApi.NetClient.GetNetworkSender<PacketId>(clientAddon);

            var go = Konpanion.knights[0];
            var control = go.GetComponent<CompanionControl>();
            // Send single data using the given packet ID
            netSender.SendSingleData(PacketId.KonpanionClientUpdate, new KonpanionClientUpdateData {
                pos = new Hkmp.Math.Vector2(go.transform.position.x,go.transform.position.y),
                anim = control.state,
                dir = control.lookDirection,
            });

        }

        private static IPacketData InstantiatePacket(PacketId packetId) {
            switch (packetId) {
                case PacketId.KonpanionServerUpdate:
                    return new KonpanionServerUpdateData();
            }
            return null;
        }
        public override void Initialize() {
            Modding.Logger.Log( "Initializing client-side KonpanionClient addon!");
            var netReceiver = clientApi.NetClient.GetNetworkReceiver<PacketId>(clientAddon, InstantiatePacket);

            netReceiver.RegisterPacketHandler<KonpanionServerUpdateData>(
                PacketId.KonpanionServerUpdate,
                packetData => {
                    var _go = Konpanion.Instance.GetNetworkKonpanion(packetData.playerId);
                    var _control = _go.GetComponent<CompanionControl>();
                    _control.state = packetData.anim;
                    _control.transform.position = new Vector2(packetData.pos.X,packetData.pos.Y);
                    _control.lookDirection = packetData.dir;
                    _control.isNetworkControlled = true;
                    _control.moveToNext = true;
                }
            );
        }

        protected override string Name => "Konpanion";
        protected override string Version => "0.0.1";
        public override bool NeedsNetwork => true;
    }
}
