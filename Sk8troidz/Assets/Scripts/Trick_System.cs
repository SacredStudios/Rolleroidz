using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick_System : MonoBehaviour
{
    [SerializeField] GameObject btn;
    public int counter;
    public bool trick_mode_activated = false;
    [SerializeField] GameObject parent;
    public void Start_Trick_System()
    {
        counter = 0;
        trick_mode_activated = true;
        StartCoroutine(Trick(3));
    }
   
    public void AddToCounter(GameObject btn)
    {
        counter++;
        Destroy(btn);
    }
    IEnumerator Trick(int n)
    {
        Vector3 position = new Vector3(0f, 0f, 0f);
        for(int i = 0; i < n; i++)
        {
            position.x += 25f;
            GameObject btn_clone = Instantiate(btn, position, Quaternion.identity, parent.transform);
            btn_clone.SetActive(true);
        }
        yield return new WaitUntil(() => counter >= n || trick_mode_activated == false);
        Debug.Log("hi");
   
    }
}
