using Godot;
using System;

public class ControllerContainer : Node {

    private PackedScene controlScene = ResourceLoader.Load<PackedScene>("res://Controller.tscn");

    private UIToggle rootController;
    private UIToggle lastController;

    private Button connectButton;
    private Button removeButton;

    public override void _Ready() {
        rootController = GetNode<UIToggle>("Controller");
        lastController = rootController;
        connectButton = GetNode<Button>("Controller/HBoxContainer/BtnConnect");
        removeButton = GetNode<Button>("Controller/HBoxContainer/BtnRemove");
    }

    public override void _Process(float delta) {
        if (connectButton.Pressed) {
            AddController();
        }
        if (removeButton.Pressed) {
            RemoveController();
        }
    }

    private void AddController() {
        lastController = (UIToggle) controlScene.Instance();
        this.AddChild(lastController);
        removeButton.Disabled = true;
        connectButton = lastController.GetNode<Button>("HBoxContainer/BtnConnect");
        removeButton = lastController.GetNode<Button>("HBoxContainer/BtnRemove");
        removeButton.Disabled = false;
    }

    private void RemoveController() {
        this.RemoveChild(lastController);
        lastController.QueueFree();
        int count = this.GetChildCount();
        lastController = this.GetChild<UIToggle>(count - 1);
        connectButton = lastController.GetNode<Button>("HBoxContainer/BtnConnect");
        removeButton = lastController.GetNode<Button>("HBoxContainer/BtnRemove");
        if (lastController != rootController) {
            removeButton.Disabled = false;
        }
    }
}
