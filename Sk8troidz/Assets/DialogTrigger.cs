using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Text_Manager tm;
    void OnTriggerEnter(Collider other)
    {
        if (!text.gameObject.activeInHierarchy) 
        {
            text.gameObject.SetActive(true);
            tm.SetText(text.gameObject);
            Invoke("HideText", 10f);
        }
    }

    public void HideText()
    {
        text.gameObject.SetActive(false);
    }
 
}
