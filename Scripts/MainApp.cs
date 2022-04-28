using Godot;
using System;

public class MainApp : Node {
    [Export]
    public int BROADCAST_PORT = 4343;
    [Export]
    public String BROADCAST_IP = "255.255.255.255";
    
    private String[] ipList;
    private PacketPeerUDP udpPeer = new PacketPeerUDP();
    private PacketPeerUDP udpList = new PacketPeerUDP();

    private bool waitingPacket = false;
    private Timer timer;

    private Godot.Collections.Dictionary<String, String> ledNames = new Godot.Collections.Dictionary<String, String>();

    public override void _Ready() {
        udpPeer.SetBroadcastEnabled(true);
        udpPeer.ConnectToHost(BROADCAST_IP, BROADCAST_PORT);
        udpList.Listen(BROADCAST_PORT);

        timer = GetNode<Timer>("Panel/Timer");
        GetNode<Button>("Panel/Button").Connect("pressed", this, nameof(SendRequest));
        timer.Connect("timeout", this, nameof(StopWaiting));
    }

    public override void _Process(float delta) {
        if (udpList.GetAvailablePacketCount() > 0) {
            byte[] pkt = udpList.GetPacket();
            if (pkt.Length == 4) {
                // packet is IP adress
                String str = "";
                for (int i = 0; i < pkt.Length; i++) {
                    int p = pkt[i];
                    str += p.ToString();
                    if (i + 1 != pkt.Length) {
                        str += ".";
                    }
                }
                GD.Print("Got: ", str);
            }
        }
    }

    private void SendRequest() {
        Error err = udpPeer.PutPacket("gip".ToUTF8());
        GD.Print("Sending UDP listen, status: ", err.ToString());
        waitingPacket = true;
        timer.Start(2);
    }

    private void StopWaiting() {
        waitingPacket = false;
        GD.Print("Finishing listening to UDP");
    }

    private void SaveConfig() {
        File cfg = new File();
        cfg.Open("user://config.cfg", File.ModeFlags.Write);

        // cfg.StoreLine();

        cfg.Close();
    }

    private void LoadConfig() {
        File cfg = new File();
        if (!cfg.FileExists("user://config.cfg")) {
            return;
        }

        cfg.Open("user://config.cfg", File.ModeFlags.Read);


        cfg.Close();
    }

}
