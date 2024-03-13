using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    //For slight shake effect at intro scene
    [SerializeField] GameObject cam;
    [SerializeField] float velocity = 0;
    [SerializeField] float acceleration;

    private void Start()
    {
   
        StartCoroutine(Transition_Down());
    }
    IEnumerator Transition_Down()
    {

        for(int i = 0; i< 100; i++)
        {
            cam.transform.Rotate(velocity,0,0);
            velocity -= acceleration;

            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 100; i++)
        {
            cam.transform.Rotate(velocity, 0, 0);
            velocity += acceleration;

            yield return new WaitForSeconds(0.01f);
        }
        //cam.transform.rotation = target_rot;
        StartCoroutine(Transition_Up());

    }
    IEnumerator Transition_Up()
    {

        for (int i = 0; i < 100; i++)
        {
            cam.transform.Rotate(velocity,0, 0);
            velocity += acceleration;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 100; i++)
        {
            cam.transform.Rotate(velocity, 0, 0);
            velocity -= acceleration;

            yield return new WaitForSeconds(0.01f);
        }
        //cam.transform.rotation = start_rot;
        StartCoroutine(Transition_Down());

    }
}
