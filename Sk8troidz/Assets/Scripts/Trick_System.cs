using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick_System : MonoBehaviour
{
    [SerializeField] GameObject btn;
    public int counter;
    public void Start_Trick_System()
    {
        counter = 0;
        StartCoroutine(Trick(3));
    }
    IEnumerator Trick(int n)
    {
        GameObject.Instantiate(btn, this.gameObject.transform);
        Vector3 position = new Vector3(0f, 0f, 0f);
        for(int i = 1; i < n; i+=2)
        {
            position.x += 25f;
            GameObject.Instantiate(btn, position, Quaternion.identity, this.gameObject.transform);
            GameObject.Instantiate(btn, -1*position, Quaternion.identity, this.gameObject.transform);
        }
        yield return new WaitUntil(() => counter >= n);
   
    }
}
