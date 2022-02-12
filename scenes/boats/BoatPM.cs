using Godot;
using System;
using FlatBuffers;
using BoatPackets;

public class BoatPM : PacketManager{

    Boat parent;

    enum METHODS_ID : byte{
        MOVEMENT,
        DESTROY
    }
    
    public override void _Ready(){
        base._Ready();
        parent = GetParent<Boat>();
    }


    public override void ReceivePacket(byte[] incoming_packet){
        ByteBuffer buffer = new ByteBuffer(GetPacketBody(incoming_packet));
        METHODS_ID method_id = (METHODS_ID) GetMethodID(incoming_packet);
        switch(method_id){
            case METHODS_ID.MOVEMENT:
                ReceiveMovement(buffer);
                break;
            case METHODS_ID.DESTROY:
                ReceiveDestroy();
                break;
        }
    }


    public void SendMovement(Vector2 position, float rotation, Vector2 velocity, float angular_velocity){
        FlatBufferBuilder builder = new FlatBufferBuilder(8);
        Movement.StartMovement(builder);
        Movement.AddPosition(builder, Vec2.CreateVec2(builder, position.x, position.y));
        Movement.AddRotation(builder, rotation);
        Movement.AddVelocity(builder, Vec2.CreateVec2(builder, velocity.x, velocity.y));
        Movement.AddAngularVelocity(builder, angular_velocity);
        var stop_building = Movement.EndMovement(builder);
        builder.Finish(stop_building.Value);
        byte[] packet = AssemblyPacket( (byte) METHODS_ID.MOVEMENT, builder.SizedByteArray());
        network.SendUnreliableToAll(packet);
    }


    void ReceiveMovement(ByteBuffer buffer){
        var table = Movement.GetRootAsMovement(buffer);
        Vector2 position = new Vector2(table.Position.Value.X, table.Position.Value.Y);
        Vector2 velocity = new Vector2(table.Velocity.Value.X, table.Velocity.Value.Y);
        parent.SyncMovement(position, table.Rotation, velocity, table.AngularVelocity);
    }


    public void SendDestroy(){
        byte[] packet = AssemblyPacket( (byte) METHODS_ID.DESTROY, new byte[0]);
        network.SendReliableToAll(packet);
    }

    void ReceiveDestroy(){
        parent.SyncDestroy();
    }

}
