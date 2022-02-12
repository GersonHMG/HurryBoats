using Godot;
using System;
using FlatBuffers;
using WeaponManagerPackets;

public class WeaponManagerPM : PacketManager{

    WeaponManager parent;

    enum METHODS_ID : byte{
        REGISTER_WEAPON,
        UNREGISTER_WEAPON,
        SELECT_WEAPON
    }


    public override void _Ready(){
        base._Ready();
        parent = GetParent<WeaponManager>();
    }


    public override void ReceivePacket(byte[] packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(packet);
        switch(method_id){
            case METHODS_ID.REGISTER_WEAPON:
                ReceiveRegisterWeapon(buffer);
                break;
            case METHODS_ID.UNREGISTER_WEAPON:
                ReceiveUnregisterWeapon(buffer);
                break;
            case METHODS_ID.SELECT_WEAPON:
                ReceiveSelectWeapon(buffer);
                break;

        }
    }


    public void SendRegisterWeapon(string weapon_name, ushort weapon_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        StringOffset name_string = builder.CreateString(weapon_name);
        RegisterWeapon.StartRegisterWeapon(builder);
        RegisterWeapon.AddWeaponName(builder, name_string);
        RegisterWeapon.AddWeaponId(builder, weapon_id);
        var stop_building = RegisterWeapon.EndRegisterWeapon(builder);
        builder.Finish(stop_building.Value);
        byte[] packet = AssemblyPacket((byte) METHODS_ID.REGISTER_WEAPON, builder.SizedByteArray());
        network.SendReliableToAll(packet);
    }


    void ReceiveRegisterWeapon(ByteBuffer buffer){
        var table = RegisterWeapon.GetRootAsRegisterWeapon(buffer);
        parent.RegisterWeapon(table.WeaponName, table.WeaponId);
    }


    // LAST WEAPON ID IS UNNECESARY !!
    public void SendSelectWeapon(ushort last_weapon_id, ushort current_weapon_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        SelectWeapon.StartSelectWeapon(builder);
        SelectWeapon.AddLastWeaponId(builder, last_weapon_id);
        SelectWeapon.AddWeaponId(builder, current_weapon_id);
        var stop_building = SelectWeapon.EndSelectWeapon(builder);
        builder.Finish(stop_building.Value);
        byte[] packet = AssemblyPacket((byte) METHODS_ID.SELECT_WEAPON, builder.SizedByteArray());
        network.SendUnreliableToAll(packet);
    }


    void ReceiveSelectWeapon(ByteBuffer buffer){
        var table = SelectWeapon.GetRootAsSelectWeapon(buffer);
        parent.SelectWeapon(table.LastWeaponId, table.WeaponId);
    }


    public void SendUnregisterWeapon(ushort weapon_id){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        UnregisterWeapon.StartUnregisterWeapon(builder);
        UnregisterWeapon.AddWeaponId(builder, weapon_id);
        var stop_building = UnregisterWeapon.EndUnregisterWeapon(builder);
        builder.Finish(stop_building.Value);
        byte[] packet = AssemblyPacket((byte) METHODS_ID.UNREGISTER_WEAPON, builder.SizedByteArray());
        network.SendReliableToAll(packet);
    }


    void ReceiveUnregisterWeapon(ByteBuffer buffer){
        var table = UnregisterWeapon.GetRootAsUnregisterWeapon(buffer);
        parent.UnregisterWeapon(table.WeaponId);
    }



}



