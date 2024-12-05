using UnityEngine;

public class HideStartMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    static bool hideMenu = false;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (hideMenu)
        {
            menu.SetActive(false);
        }
        hideMenu = true;
    }

}
