// (defrecord Snapshot
//   [^Int32 time ^GameObject gameObject ^Vector3 position ^Quaternion rotation])

using System;
using System.Collections.Generic;
using UnityEngine;

public struct Snapshot
{
  public int time;
  public GameObject gameObject;
  public Vector3 position;
  public Quaternion rotation;
  
  public Snapshot(int time, GameObject gameObject, Vector3 position, Quaternion rotation)
  {
    this.time = time;
    this.gameObject = gameObject;
    this.position = position;
    this.rotation = rotation;
  }
}

public class Snapshots
{
  public List<Snapshot> snapshots = new List<Snapshot>(500);
  
  public void Push(int time, GameObject gameObject, Vector3 position, Quaternion rotation)
  {
    snapshots.Add(new Snapshot(time, gameObject, position, rotation));
  }
}