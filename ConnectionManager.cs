using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class ConnectionManager : Node
{
  
  private HashSet<Network> Networks;
  private HashSet<Line2D> Lines;
  private VBoxContainer NetworkSelector;
  PackedScene networkOptionScene = (PackedScene)ResourceLoader.Load("res://NetworkOption.tscn");


  public override void _Ready()
  {
    base._Ready();
    Networks = new HashSet<Network>();
    Lines = new HashSet<Line2D>();
    NetworkSelector = GetNode<VBoxContainer>("/root/Game/NetworkSelector");
  }

  public void AddNetwork(Network network)
  {
    Networks.Add(network);

    NetworkOption networkOption = networkOptionScene.Instantiate<NetworkOption>();
    NetworkSelector.AddChild(networkOption);
    networkOption.SetNetwork(network);
    LineEdit nameField = networkOption.GetNode<LineEdit>("NameField");
    nameField.PlaceholderText = network.Name;

    
  }
  
}
