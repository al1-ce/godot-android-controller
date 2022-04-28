using Godot;
using System;

public class Network : Node {
    [Export]
    public int SERVER_PORT = 5001;
    [Export]
    public String SERVER_IP = "192.168.31.130";

    private int SERVER_RCP = 1;

    private StreamPeerTCP netPeer;
    private PacketPeerStream netStream;
    
    private String[] packet = new String[10];
    private int packetSize = 0;
    private bool packetFinished = true;

    private Timer timerCheckout;
    private Timer timerControl;
    private Timer timerAwait;

    public bool isConnected = false;
    private bool wasConnected = false;

    public UIToggle uitoggle;

    public enum DATA_TYPE {
        CHECKOUT = 0,
        SETLIGHT = 1,
        SETCOLOR = 2,
        SETMODE = 3,
        ENDOFPACKET = 9
    }

    public enum LIGHT_MODE {
        NONE = 0,
        RAINBOW = 1,
        FLASH = 2
    }

    public override void _Ready() {
        netPeer = new StreamPeerTCP();
        timerCheckout = GetNode<Timer>("Checkout");
        timerCheckout.Connect("timeout", this, nameof(CheckoutFail));
        timerControl = GetNode<Timer>("Control");
        timerControl.Connect("timeout", this, nameof(SendControl));
        timerAwait = GetNode<Timer>("Await");
    }

    public override void _Process(float delta) {
        wasConnected = isConnected;
        isConnected = netPeer.IsConnectedToHost();
        uitoggle.netState = isConnected;
        if (isConnected && !wasConnected) {
            GD.Print("Connected");
            timerControl.Start(delta);
        }
        if (!isConnected && wasConnected) {
            GD.Print("Disconnected");
        }

        if (isConnected) {
            if (netPeer.GetAvailableBytes() > 0) { 
                timerCheckout.Stop();
                packetSize = 0;
                while (netPeer.GetAvailableBytes() > 0) {
                    packet[packetSize] = netPeer.GetString(1);
                    //GD.Print(packet[packetSize]);
                    packetSize ++;
                }
                ProcessPacket();
            }
        }
        
    }

    public void ProcessPacket() {
        String pack = "";
        for (int i = 0; i < packetSize; i ++) {
            pack = GD.Str(pack, packet[i]);
        }
        GD.Print("Recieved: ", pack);

        if (packet[0].StartsWith(DATA_TYPE.CHECKOUT.ToString("d"))) {
            timerCheckout.Stop();//ignore
        }
        if (packet[0].StartsWith(DATA_TYPE.SETLIGHT.ToString("d"))) {
            if (packet[1].BeginsWith("0")) {
                uitoggle.lightState = false;
            } 
            if (packet[1].BeginsWith("1")) {
                uitoggle.lightState = true;
            }
        } 
        packetFinished = true;
    }

    public void SendData(DATA_TYPE flag, String data) {
        //if (timerAwait.TimeLeft != 0) return;
        //timerAwait.Start(0.1f);
        timerCheckout.Start(1);

        String pool = GD.Str(flag.ToString("d"), data, DATA_TYPE.ENDOFPACKET.ToString("d"));
        
        GD.Print("Sending: ", pool);
        netPeer.PutData(pool.ToAscii());

        // netPeer.PutData(flag.ToString("d").ToAscii());
        // netPeer.PutData(data.ToAscii());
        // netPeer.PutData(DATA_TYPE.ENDOFPACKET.ToString("d").ToAscii());
    }
    
    public void ConnectToServer() {
        if (!isConnected) {
            netPeer.ConnectToHost(SERVER_IP, SERVER_PORT); 
            GD.Print(netPeer.GetStatus());
        }
    }

    public void DisconnectFromServer() {
        if (isConnected) {
            netPeer.DisconnectFromHost();
        }
    }

    public void SetIP(String IP) {
        SERVER_IP = IP;
    }

    public void CheckoutFail() {
        DisconnectFromServer();
    }

    public void SendControl() {
        SendData(DATA_TYPE.SETLIGHT, "2");
    }
}
