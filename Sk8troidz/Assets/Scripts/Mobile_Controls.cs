using UnityEngine;

public class Mobile_Controls : MonoBehaviour
{
    void Start()
    {
       if(!Application.isMobilePlatform)
        {
            this.gameObject.SetActive(false);
        }
    }

}
