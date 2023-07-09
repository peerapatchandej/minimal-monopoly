using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxFindFirstPlayer : MonoBehaviour
{
  [SerializeField]
  private Transform image = default;

  public void Setup(Action onComplete = null)
  {
    image.DOBlendableLocalMoveBy(new Vector2(0, -200f), 0.3f).OnComplete(() =>
    {
      onComplete?.Invoke();
    });
  }
}
