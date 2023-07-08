using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
  [SerializeField]
  private Button createRoom = default;

  [SerializeField]
  private Button quitGame = default;

  public void Setup(ResourceLoader resourceLoader, Action onCreateMainMenu, Action<List<int>> onLoadGameScene)
  {
    if (createRoom)
    {
      createRoom.onClick.AddListener(() =>
      {
        resourceLoader.LoadAndCreateUI("CreateRoom", (obj) =>
        {
          UICreateRoom createRoom = obj.GetComponent<UICreateRoom>();
          if (createRoom)
          {
            createRoom.Setup(onCreateMainMenu, onLoadGameScene);
          }
        });
        createRoom.interactable = false;
        Destroy(gameObject);
      });
    }
    if (quitGame)
    {
      quitGame.onClick.AddListener(() =>
      {
        quitGame.interactable = false;
        Application.Quit();
      });
    }
  }
}
