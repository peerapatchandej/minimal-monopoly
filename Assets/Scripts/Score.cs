using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
  [SerializeField]
  private Image crown = default;

  [SerializeField]
  private TextMeshProUGUI player = default;

  [SerializeField]
  private TextMeshProUGUI no = default;

  public void Setup(int no, Color color, string playerName)
  {
    crown.color = new Color(crown.color.r, crown.color.g, crown.color.b, no == 1 ? 1 : 0);
    player.color = color;
    player.text = playerName;

    switch (no)
    {
      case 1:
        this.no.text = $"1st";
        break;
      case 2:
        this.no.text = $"2nd";
        break;
      case 3:
        this.no.text = $"3rd";
        break;
      case 4:
        this.no.text = $"4th";
        break;
    }
  }
}