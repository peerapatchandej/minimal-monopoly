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

  public void Setup(ResourceLoader resourceLoader)
  {
    if (createRoom)
    {
      createRoom.onClick.AddListener(() =>
      {
        resourceLoader.LoadAndCreateUI("CreateRoom", (obj) =>
        {

        });
        createRoom.interactable = false;
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
