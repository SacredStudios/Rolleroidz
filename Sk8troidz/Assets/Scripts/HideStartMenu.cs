using UnityEngine;

public class HideStartMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject rolleroidMenu;
    static bool hideMenu = false;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (hideMenu)
        {
            menu.SetActive(false);
            rolleroidMenu.GetComponent<MenuSk8troid>().Spin_Enabled();
        }
        hideMenu = true;
    }

}
