using Godot;
using System;
using System.Collections.Generic;

public partial class Station : Node2D
{

  private bool Clicked = false;
  private static int _idCounter = 0;
  public int StationID { get; private set; }
  public HashSet<Station> ConnectedStations { get; private set; }
  private Dictionary<int, Line2D> ConnectionLines;
  private Game GameInstance;
  private Vector2 LastPosition;

  [Signal]
  public delegate void StationMovedEventHandler(Vector2 newPosition);


  public override void _Ready()
  {
    base._Ready();
    GameInstance = GetNode<Game>("/root/Game");

    StationID = _idCounter;
    _idCounter++;

    ConnectedStations = new HashSet<Station>();
    ConnectionLines = new Dictionary<int, Line2D>();

    GetNode<Label>("./Label").Text = "Station " + (StationID + 1).ToString();
    LastPosition = Position;
  }

  public override void _Process(double delta)
  {

    Vector2 mousePosition = GetViewport().GetMousePosition();

    if (Clicked)
    {
      Position = mousePosition;
    }

    if (Position != LastPosition)
    {
      EmitSignal(SignalName.StationMoved, Position);
    }
  }

  public void SetClicked(bool input)
  {
    Clicked = input;
  }


  public bool PointInside(Vector2 point)
  {
    double distanceFromCenter = Math.Sqrt(Math.Pow(point.X - Position.X, 2) + Math.Pow(point.Y - Position.Y, 2));

    if (distanceFromCenter <= 32)
    {
      return true;
    }
    return false;
  }

  public bool IsConnectedTo(Station station)
  {
    return ConnectedStations.Contains(station);
  }

  public void ConnectStation(Station station)
  {
    ConnectedStations.Add(station);
    if (station.StationID < StationID)
    {
      Line2D line = new Line2D();
      line.Points = [
        new Vector2(0, 0),
        ToLocal(station.Position)
      ];

      line.Width = 10;
      line.DefaultColor = Colors.Fuchsia;
      line.EndCapMode = Line2D.LineCapMode.Round;

      line.ZIndex = -1;

      AddChild(line);
      line.Name = "Station" + (station.StationID + 1).ToString() + "Line";

      ConnectionLines.Add(station.StationID, line);
    }
  }

  public void DisconnectStation(Station station)
  {
    ConnectedStations.Remove(station);
    
    if (station.StationID < StationID)
    {
      ConnectionLines.Remove(station.StationID);
      Line2D line = GetNode<Line2D>("Station" + (station.StationID + 1).ToString() + "Line");
      line.QueueFree();
    }
  }

  public void DeleteStation()
  {
    foreach (Station station in ConnectedStations)
    {
      station.DisconnectStation(this);
    }
    QueueFree();
  }
}
