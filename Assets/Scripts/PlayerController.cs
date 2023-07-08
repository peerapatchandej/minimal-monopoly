using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
  public int currentIndex { get; private set; }

  private int health = Const.DEFAULT_HEALTH;
  private float moveSpeed = 0.5f;

  public void Setup(int index, int health = 0)
  {
    if (health != 0) this.health = health;
    SetIndex(index);
  }

  public void Move(int index, Vector2 des, Action onComplete = null)
  {
    SetIndex(index);
    transform.DOMove(des, moveSpeed).OnComplete(() =>
    {
      onComplete?.Invoke();
    });
  }

  private void SetIndex(int index)
  {
    currentIndex = index;
  }
}
