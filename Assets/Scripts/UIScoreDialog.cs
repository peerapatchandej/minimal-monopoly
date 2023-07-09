using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDialog : MonoBehaviour
{
  [SerializeField]
  private GameObject scoreListObj = default;

  [SerializeField]
  private Transform parent = default;

  [SerializeField]
  private  Transform panel = default;

  [SerializeField]
  private Button rematch = default;

  [SerializeField]
  private Button mainMenu = default;

  private IEnumerator Start()
  {
    yield return new WaitForEndOfFrame();
    panel.DOScale(1, 0.3f);
  }

  public void Setup(List<PlayerColor> scoreList, Action onRematch, Action onMainMenu)
  {
    for (int i = 0; i < scoreList.Count; i++)
    {
      GameObject obj = Instantiate(scoreListObj, parent);
      Score score = obj.GetComponent<Score>();
      Color color = default;
      string playerName = "";

      switch (scoreList[i])
      {
        case PlayerColor.Red:
          color = Const.RED_COLOR;
          playerName = "Red Player";
          break;
        case PlayerColor.Blue:
          color = Const.BLUE_COLOR;
          playerName = "Blue Player";
          break;
        case PlayerColor.Yellow:
          color = Const.YELLOW_COLOR;
          playerName = "Yellow Player";
          break;
        case PlayerColor.Green:
          color = Const.GREEN_COLOR;
          playerName = "Green Player";
          break;
      }

      score.Setup(i + 1, color, playerName);
    }

    rematch.onClick.AddListener(() =>
    {
      rematch.interactable = false;
      onRematch?.Invoke();
    });

    mainMenu.onClick.AddListener(() =>
    {
      mainMenu.interactable = false;
      onMainMenu?.Invoke();
    });
  }
}