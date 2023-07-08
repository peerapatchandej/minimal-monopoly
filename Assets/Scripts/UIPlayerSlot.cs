using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSlot : MonoBehaviour
{
  [SerializeField]
  private GameObject EmptyObj = default;

  [SerializeField]
  private GameObject pawnObj = default;

  [SerializeField]
  private GameObject aiLabel = default;

  [SerializeField]
  private Button joinSlot = default;

  [SerializeField]
  private Button addAi = default;

  [SerializeField]
  private Button remove = default;

  public void Setup(Action<int> onJoin, Action<int> onAddAi, Action<int> onRemove)
  {
    JoinSlot(false);
    aiLabel.SetActive(false);

    joinSlot.onClick.AddListener(() =>
    {
      JoinSlot(true);
      onJoin?.Invoke(transform.GetSiblingIndex());
    });
    
    addAi.onClick.AddListener(() =>
    {
      JoinSlot(true);
      aiLabel.SetActive(true);
      onAddAi?.Invoke(transform.GetSiblingIndex());
    });
    
    remove.onClick.AddListener(() =>
    {
      JoinSlot(false);
      aiLabel.SetActive(false);
      onRemove?.Invoke(transform.GetSiblingIndex());
    });
  }

  private void JoinSlot(bool value)
  {
    pawnObj.SetActive(value);
    EmptyObj.SetActive(!value);
    addAi.gameObject.SetActive(!value);
    joinSlot.gameObject.SetActive(!value);
    remove.gameObject.SetActive(value);
  }
}
