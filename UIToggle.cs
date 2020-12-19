using Godot;
using System;

public class UIToggle : Control {

    private PackedScene networkScene = (PackedScene) ResourceLoader.Load("res://Network.tscn");
    private Network network;

    private String ip = "192.168.31.130";

    private Button btnEditIP;
    private Button btnConnect;

    private Panel pnlConnect;
    private LineEdit editIP;
    private Button btnAccept;

    private Panel pnlController;
    private Button btnRefresh;
    private Button btnToggle;
    private Button btnStatus;
    private Button btnHide;
    
    public bool lightState = false;
    public bool netState = false;


    private StyleBox colRed = (StyleBox) ResourceLoader.Load("res://BtnStatusColorRed.tres");
    private StyleBox colGreen = (StyleBox) ResourceLoader.Load("res://BtnStatusColorGreen.tres");

    public override void _Ready() {
        network = (Network) networkScene.Instance();
        this.AddChild(network);
        network.uitoggle = this;

        btnEditIP = GetNode<Button>("BtnEditIP");
        btnConnect = GetNode<Button>("BtnConnect");

        btnEditIP.Connect("pressed", this, nameof(ShowEditIP));
        btnConnect.Connect("pressed", this, nameof(ConnectController));
        
        pnlConnect = GetNode<Panel>("PnlConnect");
        editIP = GetNode<LineEdit>("PnlConnect/EditIP");
        btnAccept = GetNode<Button>("PnlConnect/BtnAccept");
        
        editIP.Text = "192.168.31.130";
        btnAccept.Connect("pressed", this, nameof(SetNewIP));
        
        pnlController = GetNode<Panel>("PnlController");
        btnHide = GetNode<Button>("PnlController/BtnHide");
        btnRefresh = GetNode<Button>("PnlController/BtnRefresh");
        btnStatus = GetNode<Button>("PnlController/BtnStatus");
        btnToggle = GetNode<Button>("PnlController/BtnToggle");
        
        btnHide.Connect("pressed", this, nameof(HideController));
        btnRefresh.Connect("pressed", this, nameof(RefreshNetwork));
        btnToggle.Connect("pressed", this, nameof(ToggleLight));
    }

    public override void _Process(float delta) {
        if (lightState) {
            btnToggle.Text = "Turn off";
        } else {
            btnToggle.Text = "Turn on";
        }

        if (netState) {
            btnRefresh.Disabled = true;
            btnToggle.Disabled = false;
            btnStatus.Set("custom_styles/normal", colGreen);
            btnStatus.Set("custom_styles/hover", colGreen);
        } else {
            btnRefresh.Disabled = false;
            btnToggle.Disabled = true;
            btnStatus.Set("custom_styles/normal", colRed);
            btnStatus.Set("custom_styles/hover", colRed);
        }
    }

    public void ShowEditIP() {
        pnlConnect.Show();
    }

    public void ConnectController() {
        network.ConnectToServer();
        pnlController.Show();
    }

    public void SetNewIP() {
        ChangeIP(editIP.Text);
        pnlConnect.Hide();
    }

    public void HideController() {
        pnlController.Hide();
        network.DisconnectFromServer();
    }

    public void RefreshNetwork() {
        network.ConnectToServer();
    }

    public void ToggleLight() {
        network.SendData(Network.DATA_TYPE.SETLIGHT, lightState ? "0" : "1");
    }

    public void ChangeIP(String IP) {
        network.SetIP(IP);
    }

}
