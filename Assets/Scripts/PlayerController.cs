using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
  public int currentIndex { get; private set; }

  private float moveSpeed = 1f;

  public void Setup(int index)
  {
    SetIndex(index);
  }

  public void Move(int index, Vector2 des)
  {
    SetIndex(index);
    transform.DOMove(des, moveSpeed);
  }

  private void SetIndex(int index)
  {
    currentIndex = index;
  }
}
