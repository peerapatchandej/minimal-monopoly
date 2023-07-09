using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceTypeSetting : SettingMenu
{
  [SerializeField]
  private int[] dices = default;

  private void Awake()
  {
    SetText();
    UpdateButton();

    reduce.onClick.AddListener(() =>
    {
      if (currentValue == 6) currentValue = 4;
      else if (currentValue == 8) currentValue = 6;
      else if (currentValue == 10) currentValue = 8;
      else if (currentValue == 12) currentValue = 10;

      SetText();
      UpdateButton();
    });

    add.onClick.AddListener(() =>
    {
      if (currentValue == 4) currentValue = 6;
      else if (currentValue == 6) currentValue = 8;
      else if (currentValue == 8) currentValue = 10;
      else if (currentValue == 10) currentValue = 12;

      SetText();
      UpdateButton();
    });
  }

  protected override void SetText()
  {
    valueText.text = $"D{currentValue}";
  }
}