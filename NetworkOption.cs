using Godot;
using System;

public partial class NetworkOption : Control
{
  private Network Network;

  public void SetNetwork(Network network)
  {
    Network = network;
  }

  public Network GetNetwork()
  {
    return Network;
  }

  private void OnLineEditTextSubmitted()
  {
    LineEdit nameField = GetNode<LineEdit>("./NameField");
    Network.SetName(nameField.Text);
  }
}
