using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node2D
{
  public Dictionary<int, Station> StationRegistry {get; private set;}
  
  [Export]
  public PackedScene StationScene { get; set; }


  private Station connectionStart;
  private Station connectionEnd;
  private bool connectionMode = false;

  private ConnectionManager connectionManager;

  private Random rng;

  public override void _Ready()
  {
    base._Ready();
    StationRegistry = new Dictionary<int, Station>();
    connectionManager = GetNode<ConnectionManager>("./ConnectionManager");
    rng = new Random();
  }
  public override void _Process(double delta)
  {
    Vector2 mousePosition = GetViewport().GetMousePosition();
    HandleInput(mousePosition);

    QueueRedraw();  
  }

  public override void _Draw()
  {
    Vector2 mousePosition = GetViewport().GetMousePosition();
    base._Draw();

    if (connectionMode)
    {
      DrawLine(connectionStart.Position, mousePosition, Colors.Green, 10.0f);
    }

  }


  private void SpawnNewStation(Vector2 position)
  {
    Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
    Station station = StationScene.Instantiate<Station>();
    station.Position = new Vector2(viewportSize.X / 2, viewportSize.Y / 2);
    AddChild(station);
    StationRegistry.Add(station.StationID, station);
    station.Position = position;
  }

  private void HandleInput(Vector2 mousePosition)
  {
    if (Input.IsActionJustPressed("new_station"))
    {
      SpawnNewStation(mousePosition);
    }

    if (Input.IsActionJustPressed("left_click"))
    {
      foreach (Station station in StationRegistry.Values)
      {
        if (station.PointInside(mousePosition))
        {
          if (connectionMode)
          {
            connectionEnd = station;
            EndConnection();
            break;
          }
          station.SetClicked(true);
          break;
        }
      }
      // even if we don't click a station, connection mode needs to be exited
      if (connectionMode)
      {
        connectionMode = false;
      }
    }

    if (Input.IsActionJustReleased("left_click"))
    {
      foreach (Station station in StationRegistry.Values)
      {
        if (station.PointInside(mousePosition))
        {
          station.SetClicked(false);
        }
      }
    }

    if (Input.IsActionJustReleased("delete_station"))
    {
      foreach (Station station in StationRegistry.Values)
      {
        if (station.PointInside(mousePosition))
        {
          StationRegistry.Remove(station.StationID);
          station.DeleteStation();
          break;
        }
      }
    }

    if (Input.IsActionJustReleased("connect_nodes"))
    {
      foreach (Station station in StationRegistry.Values)
      {
        if (station.PointInside(mousePosition))
        {
          GD.Print("Starting connection from " + station.GetNode<Label>("./Label").Text);
          connectionStart = station;
          connectionMode = true;
          break;
        }
      }
    }

    if (Input.IsActionJustPressed("new_network"))
    {
      byte[] bytes = new byte[4];
      rng.NextBytes(bytes);
      bytes[3] = 0xFF;
      uint randomUInt = BitConverter.ToUInt32(bytes, 0);
      Color color = new Color(randomUInt);
      Network network = new Network(color);

      connectionManager.AddNetwork(network);
    }
  }

  private void EndConnection()
  {
    connectionMode = false;
    if (connectionEnd.IsConnectedTo(connectionStart))
    {
      GD.Print("Disconnected " + connectionStart.GetNode<Label>("./Label").Text + " from " + connectionEnd.GetNode<Label>("./Label").Text);
      connectionEnd.DisconnectStation(connectionStart);
      connectionStart.DisconnectStation(connectionEnd);
    }
    else
    {
      GD.Print("Connected " + connectionStart.GetNode<Label>("./Label").Text + " to " + connectionEnd.GetNode<Label>("./Label").Text);
      connectionEnd.ConnectStation(connectionStart);
      connectionStart.ConnectStation(connectionEnd);
    }
  }
}
