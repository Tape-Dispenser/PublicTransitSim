using System.Collections.Generic;
using System.Numerics;
using Godot;

public class Network
{
  private static int _idIndex = 0;
  public int NetworkID;
  private HashSet<Station> ConnectedStations;
  private HashSet<Pair<Station>> Connections;
  public Color Color {get; private set;}
  public string Name {get; private set;}

  public Network(Color color)
  {
    ConnectedStations = new HashSet<Station>();
    Color = color;

    NetworkID = _idIndex;
    _idIndex++;

    Name = "Line " + (NetworkID + 1).ToString();
  }

  public void AddStation(Station station)
  {
    ConnectedStations.Add(station);
  }

  public void AddConnection(Station start, Station end)
  {
    Connections.Add(new Pair<Station>(start, end));
  }

  public void SetName(string name)
  {
    Name = name;
  }
}