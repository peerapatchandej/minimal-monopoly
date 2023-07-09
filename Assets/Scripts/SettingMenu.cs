using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingMenu : MonoBehaviour
{
  [SerializeField]
  protected Button reduce = default;

  [SerializeField]
  protected Button add = default;

  [SerializeField]
  protected TextMeshProUGUI valueText = default;

  [SerializeField]
  protected int minValue = default;

  [SerializeField]
  protected int maxValue = default;

  [SerializeField]
  protected int currentValue = default;

  private void Awake()
  {
    SetText();
    UpdateButton();

    reduce.onClick.AddListener(() =>
    {
      currentValue--;
      SetText();
      UpdateButton();
    });

    add.onClick.AddListener(() =>
    {
      currentValue++;
      SetText();
      UpdateButton();
    });
  }

  public int GetValue()
  {
    return currentValue;
  }

  protected virtual void SetText()
  {
    valueText.text = currentValue.ToString();
  }

  protected void UpdateButton()
  {
    reduce.interactable = currentValue > minValue;
    add.interactable = currentValue < maxValue;
  }
}