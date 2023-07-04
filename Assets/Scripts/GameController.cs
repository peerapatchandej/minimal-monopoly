using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  private ResourceLoader resourceLoader;

  private void Awake()
  {
    resourceLoader = new ResourceLoader();
    DontDestroyOnLoad(gameObject);
  }

  private void Start()
  {
    resourceLoader.LoadAndCreateUI("MainMenu", (obj) =>
    {
      UIMainMenu mainMenu = obj.GetComponent<UIMainMenu>();
      if (mainMenu)
      {
        mainMenu.Setup(resourceLoader);
      }
    });
  }
}