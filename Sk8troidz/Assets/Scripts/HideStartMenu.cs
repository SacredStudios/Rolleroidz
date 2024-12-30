using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HideStartMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject rolleroidMenu;
    [SerializeField] GameObject menuController;
    static bool hideMenu = false;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (hideMenu)
        {
            menu.SetActive(false);
            rolleroidMenu.GetComponent<MenuSk8troid>().Spin_Enabled();
            menuController.GetComponent<WordJumper>().uiElements = new RectTransform[0];
        }
        hideMenu = true;
    }

}
