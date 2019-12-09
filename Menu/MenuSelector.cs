using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    public List<MenuButton> MenuButtons;
    public GameObject lastMenu;


    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        foreach(MenuButton menuButton in MenuButtons)
        {
            menuButton.Menu.SetActive(lastMenu == menuButton.Menu);
            menuButton.button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                lastMenu?.SetActive(false);
                lastMenu = menuButton.Menu;
                lastMenu?.SetActive(true);
            }));
        }
    }

}

[System.Serializable]
public class MenuButton
{
    public Button button;
    public GameObject Menu;


}
