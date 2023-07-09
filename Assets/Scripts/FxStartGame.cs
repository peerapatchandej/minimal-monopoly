using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class FxStartGame : MonoBehaviour
{
  [SerializeField]
  private Transform image = default;

  public void Setup(Action onComplete = null)
  {
    image.DOScaleX(1, 0.3f).OnComplete(() =>
    {
      onComplete?.Invoke();
    });
  }
}