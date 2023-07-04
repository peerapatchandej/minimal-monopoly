using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader
{
  public void LoadAndCreateUI(string name, Action<GameObject> callback)
  {
    ResourceRequest resourceRequest = Resources.LoadAsync($"Prefabs/{name}");
    resourceRequest.completed += (o) =>
    {
      if (resourceRequest.asset != default)
      {
        GameObject obj = UnityEngine.Object.Instantiate((GameObject)resourceRequest.asset);
        callback?.Invoke(obj);
      }
    };
  }
}
