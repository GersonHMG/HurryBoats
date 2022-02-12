// Deserialize, Serialize and Send packets

using Godot;
using System;
using System.Text;
using System.Linq;


public class PacketManager : Node {

    protected Network network;
    protected LobbyManager lobby_manager;
    ulong client_id;
    protected PlayerLobbyData client_data;

    string node_identifier = "";
    ushort node_identifier_size = 1;
    byte[] packet_header;

    
    
    public override void _Ready(){
        network = GetTree().Root.GetNode<Network>("Network");
        lobby_manager = GetNode<LobbyManager>("/root/LobbyManager");
        if(lobby_manager.IsLobbyActive()){
            client_id = (ulong) lobby_manager.GetMySteamID();
            client_data = lobby_manager.GetMyData();
        }
        node_identifier = GetPath();
        packet_header = MakePacketHeader(node_identifier);
        network.RegistryNode( node_identifier, this);
    }


    byte[] MakePacketHeader(string node_i){
        byte[] identifier = Encoding.ASCII.GetBytes(node_i);
        node_identifier_size = (ushort) identifier.Length;
        byte[] size = BitConverter.GetBytes( node_identifier_size );
        byte[] final_header = Concat(size,identifier);
        //GD.Print(" Header length: " + final_header.Length.ToString() );
        return final_header;
     }


    public virtual void ReceivePacket(byte[] packet){
    }


    // [NodeIdentifierSize (2 bytes)] [NodeIdentifier 0...n bytes] [method_id (2 bytes)] [methods args...]
    protected byte[] AssemblyPacket(byte destination_method, byte[] packet){
        byte[] method_id = new byte[1];
        method_id[0] = destination_method;
        byte[] body = Concat(method_id, packet);
        byte[] final_packet = Concat(packet_header, body);
        return final_packet;
    }

    protected byte GetMethodID(byte[] packet){
        byte method_id = packet[0];
        return method_id;
    }

    protected byte[] GetPacketBody(byte[] packet){
        return packet.Skip(1).ToArray<byte>();
    }


    byte[] Concat(byte[] a, byte[] b){           
        byte[] output = new byte[a.Length + b.Length];
        for (int i = 0; i < a.Length; i++)
            output[i] = a[i];
        for (int j = 0; j < b.Length; j++)
            output[a.Length+j] = b[j];
        return output;           
    }


    public ulong GetClientID() => client_data.GetPlayerID();

    public bool ImHost() => lobby_manager.ImHost();

    public PlayerLobbyData GetClientData() => client_data;

}
