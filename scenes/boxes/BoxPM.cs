using Godot;
using System;

public class BoxPM : PacketManager{

    Box parent;

    enum METHODS_ID : byte{
        DESTROY
    }


    public override void _Ready(){
        base._Ready();
        parent = GetParent<Box>();
    }


    public override void ReceivePacket(byte[] packet){
        base.ReceivePacket(packet);
        METHODS_ID method_id = (METHODS_ID) GetMethodID(packet);
        
        switch(method_id){
            case METHODS_ID.DESTROY:
                ReceiveDestroyCrate();
                break;
        }
    }


    public void SendDestroyCrate(){
        byte[] packet = AssemblyPacket( (byte) METHODS_ID.DESTROY, new byte[0]);
        network.SendReliableToAll(packet);
    }

    
    void ReceiveDestroyCrate(){
        GD.Print("BOX DESTROYED BY SERVER");
        parent.SyncDestroyCrate();
    }

}
