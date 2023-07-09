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

  [SerializeField]
  private Image fxTakeDamage = default;

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
    fxUpgrade.gameObject.SetActive(true);
    fxUpgrade.color = GetColor();
    fxUpgrade.DOFade(0f, 1f);
    fxUpgrade.transform.DOLocalMoveY(113f, 1f).OnComplete(() =>
    {
      fxUpgrade.transform.localPosition = Vector3.zero;
    });
  }

  public void TakeDamage()
  {
    fxTakeDamage.gameObject.SetActive(true);
    fxTakeDamage.color = GetColor();
    fxTakeDamage.DOFade(0f, 1f);
    fxTakeDamage.transform.DOLocalMoveY(113f, 1f).OnComplete(() =>
    {
      fxTakeDamage.transform.localPosition = Vector3.zero;
    });
  }

  private Color GetColor()
  {
    switch (playerColor)
    {
      case PlayerColor.Red:
        return Const.RED_COLOR;
      case PlayerColor.Blue:
        return Const.BLUE_COLOR;
      case PlayerColor.Yellow:
        return Const.YELLOW_COLOR;
      case PlayerColor.Green:
        return Const.GREEN_COLOR;
    }

    return Color.white;
  }

  private void SetIndex(int index)
  {
    currentIndex = index;
  }
}
