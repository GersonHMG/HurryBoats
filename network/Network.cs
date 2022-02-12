// Packet Transport
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Steamworks;
using System.Text;

public class Network : Node {

    LobbyManager lobby_manager;
    Dictionary<string, PacketManager> network_node_by_id = new Dictionary<string, PacketManager>();
    
    
    public override void _Ready(){
        lobby_manager = GetNode<LobbyManager>("/root/LobbyManager");
    }


    public override void _Process(float delta){
        ReceivePackets();
    }


    private void ReceivePackets(){
        while (SteamNetworking.IsP2PPacketAvailable(out uint packet_size)){
            byte[] incoming_packet = new byte[packet_size];
            if (SteamNetworking.ReadP2PPacket(incoming_packet, packet_size, out uint bytesRead, out CSteamID remoteID)){
                string destination = GetIdentifierFromPacket(incoming_packet);
                int header_size = GetHeaderSizeFromPacket(incoming_packet);
                byte[] delivery_packet = GetPacketBody(header_size, incoming_packet);
                DeliveryPacket(destination, delivery_packet);
            }
        }
    }


    string GetIdentifierFromPacket(byte[] packet){
        ushort identifier_size = BitConverter.ToUInt16(packet.Take(2).ToArray<byte>(), 0);
        string identifier = Encoding.ASCII.GetString( packet.Skip(2).Take(identifier_size).ToArray<byte>() );
        return identifier;
    }


    int GetHeaderSizeFromPacket(byte[] packet){
        ushort identifier_size = BitConverter.ToUInt16(packet.Take(2).ToArray<byte>(), 0);
        int header_size = 2 + identifier_size; // ushort + identifier_size
        return header_size;
    }


    byte[] GetPacketBody(int header_size, byte[] packet){
        byte[] body = packet.Skip( header_size ).Take( packet.Length + header_size ).ToArray<byte>();
        return body;
    }


    void DeliveryPacket(string destination, byte[] packet_body){
        PacketManager value_out;
        if(network_node_by_id.TryGetValue(destination,out value_out)){
            //if(IsInstanceValid(value_out))
                value_out.ReceivePacket(packet_body);
        }
        else{
            GD.Print(" Failed to access node: " + destination);
        }
    }


    public void RegistryNode(string identifier, PacketManager node){
        PacketManager value_out;
        if(network_node_by_id.TryGetValue(identifier,out value_out)){
            GD.Print("Failed to registry node, node already exists: " + identifier);
        }
        else{
            network_node_by_id.Add(identifier, node);

            node.Connect("tree_exited", this, nameof(UnregisterNode),new Godot.Collections.Array(){identifier} );
        }
    }


    void UnregisterNode(string identifier){
        network_node_by_id.Remove(identifier);
    }



    void SendPacket(CSteamID player_id, byte[] packet, EP2PSend mode){
        SteamNetworking.SendP2PPacket(player_id, packet, (uint) packet.Length, mode);
    }



    public void SendReliableToAll(byte[] packet){
        CSteamID my_id = lobby_manager.GetMySteamID();
        foreach(CSteamID player_id in lobby_manager.GetPlayersSteamID() ){

            if(player_id != my_id){

                SendPacket(player_id, packet, EP2PSend.k_EP2PSendReliable);

            }
        } 
    }
 

    public void SendReliableToID(byte[] packet, CSteamID player_id){
        SendPacket(player_id, packet, EP2PSend.k_EP2PSendReliable);
    }


    public void SendUnreliableToAll(byte[] packet){
        CSteamID my_id = lobby_manager.GetMySteamID();
        foreach(CSteamID player_id in lobby_manager.GetPlayersSteamID() ){
            if(player_id != my_id){
                SendPacket(player_id, packet, EP2PSend.k_EP2PSendUnreliable);
            }
        } 
    }


    public void SendToAll(byte[] packet){
        foreach(CSteamID player_id in lobby_manager.GetPlayersSteamID() ){
                SendPacket(player_id, packet, EP2PSend.k_EP2PSendUnreliable);
        } 
    }

    public void SendUnReliableToID(){

    }

}