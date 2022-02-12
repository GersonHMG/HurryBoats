using Godot;
using System;

public class WeaponPM : PacketManager{

    DoubleCannons parent;

    enum METHODS_ID : byte{
        SHOOT
    }

    public override void _Ready(){
        base._Ready();
        parent = GetParent<DoubleCannons>();
    }

    public override void ReceivePacket(byte[] packet){
        base.ReceivePacket(packet);
        METHODS_ID method_id = (METHODS_ID) GetMethodID(packet);
        switch(method_id){
            case METHODS_ID.SHOOT:
                ReceiveShoot();
                break;
        }
    }


    public void SendShoot(){
        byte[] packet = AssemblyPacket((byte) METHODS_ID.SHOOT, new byte[0]);
        network.SendReliableToAll(packet);
    }


    void ReceiveShoot(){
        parent.Shoot();
    }

}
