using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
  private void Awake()
  {
    Setup();
  }

  public void Setup()
  {
    var canvas = GetComponent<Canvas>();
    canvas.worldCamera = Camera.main;
  }
}
