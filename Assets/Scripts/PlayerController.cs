using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
  [SerializeField]  //temporary
  public int currentIndex;

  [SerializeField]
  private int health = Const.DEFAULT_HEALTH;

  [SerializeField]
  private Image image = default;

  public PlayerType playerType { get; private set; }
  public int conerIndex { get; private set; }

  private float moveSpeed = 0.5f;
  private bool playerLose = false;

  public void Setup(PlayerType playerType, int health, int index)
  {
    this.playerType = playerType;
    this.health = health;
    conerIndex = index;
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

  public void UpdateHealth(int health, Action onLose)
  {
    this.health += health;
    if (this.health <= 0)
    {
      playerLose = true;
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
      onLose?.Invoke();
    }
  }

  public int GetHealth()
  {
    return health;
  }

  public bool PlayerLose()
  {
    return playerLose;
  }

  private void SetIndex(int index)
  {
    currentIndex = index;
  }
}
