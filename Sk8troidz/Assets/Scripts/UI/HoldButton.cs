using UnityEngine;

public class HoldButton : MonoBehaviour
{
    public bool isDown;
    public void Btn_Up()
    {
        isDown = false;
    }
    public void Btn_Down()
    {
        isDown = true;
    }
}
