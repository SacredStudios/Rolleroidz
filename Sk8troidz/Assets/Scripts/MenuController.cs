using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 target_pos;
    [SerializeField] Vector3 start_pos;

    [SerializeField] GameObject StartBtn;
    [SerializeField] GameObject Menu_Skatroid;
    [SerializeField] GameObject StartMenu;


    public void ShowMainMenu()
    {
        StartBtn.GetComponent<Button>().enabled = false;
        StartCoroutine(Transition_Down());
     
        Invoke("HideStartMenu", 1f);
        
    }
  void HideStartMenu()
    {
        StartMenu.SetActive(false);
        Menu_Skatroid.SetActive(true);
    }

    IEnumerator Transition_Down()
    {
        transition.transform.position = start_pos;
        while (transition.transform.position.y > target_pos.y)
        {
            transition.transform.position -= velocity;

            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Transition_Up());

    }
    IEnumerator Transition_Up()
    {
        transition.transform.position = target_pos;
        while (transition.transform.position.y < start_pos.y)
        {
            transition.transform.position += velocity;

            yield return new WaitForSeconds(0.01f);
        }
        



    }
}
