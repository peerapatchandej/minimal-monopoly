using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
  public int originIndex;
  public int currentIndex { get; private set; }
  public int ownedSlot;   //อาจจะแก้เป็น enum slot type จะดีกว่า

  private float moveSpeed = 0.5f;

  public void Setup(int index)
  {
    SetIndex(index);
    originIndex = index;
    ownedSlot = 4;
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
