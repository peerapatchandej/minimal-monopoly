using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMainMenu : MonoBehaviour
{
  private ResourceLoader resourceLoader = new ResourceLoader();

  private void Awake()
  {
    CreateMainMenu();
  }

  private void CreateMainMenu()
  {
    resourceLoader.LoadAndCreateUI("MainMenu", (obj) =>
    {
      UIMainMenu mainMenu = obj.GetComponent<UIMainMenu>();
      if (mainMenu)
      {
        mainMenu.Setup(resourceLoader, CreateMainMenu, (players) =>
        {

        });
      }
    });
  }
}
