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

  [SerializeField]
  private Image fxUpgrade = default;

  public PlayerColor playerColor { get; private set; }
  public PlayerType playerType { get; private set; }
  public int conerIndex { get; private set; }

  private float moveSpeed = 0.3f;
  private bool playerLose = false;

  public void Setup(PlayerColor playerColor, PlayerType playerType, int health, int index)
  {
    this.playerColor = playerColor;
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

  public void UpgradeSlot()
  {
    Color color = default;

    switch (playerColor)
    {
      case PlayerColor.Red:
        color = Const.RED_COLOR;
        break;
      case PlayerColor.Blue:
        color = Const.BLUE_COLOR;
        break;
      case PlayerColor.Yellow:
        color = Const.YELLOW_COLOR;
        break;
      case PlayerColor.Green:
        color = Const.GREEN_COLOR;
        break;
    }

    fxUpgrade.gameObject.SetActive(true);
    fxUpgrade.color = color;
    fxUpgrade.DOFade(0f, 1f);
    fxUpgrade.transform.DOLocalMoveY(113f, 1f).OnComplete(() =>
    {
      fxUpgrade.transform.localPosition = Vector3.zero;
    });
  }

  private void SetIndex(int index)
  {
    currentIndex = index;
  }
}
