using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI[] healthText = default;

  public void Setup(List<int> playerSlotIndex)
  {
    for (int i = 0; i < playerSlotIndex.Count; i++)
    {
      healthText[playerSlotIndex[i]].gameObject.SetActive(true);
    }
  }

  public void SetHealth(int index, int health)
  {
    healthText[index].text = $"Health : {health}";
  }
}
