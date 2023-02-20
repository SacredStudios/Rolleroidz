using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] float trans_speed;
    [SerializeField] float max_trans_speed;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 target_pos;
    [SerializeField] Vector3 start_pos;
    [SerializeField] float smooth_time;
    [SerializeField] bool going_down;
    public void Transition_Up()
    {
       // going_down = false;

    }
    void Transition_Down()
    {
        going_down = true;

    }
    private void FixedUpdate()
    {
        if(going_down)
        {
            transition.transform.position = Vector3.SmoothDamp(transition.transform.position, new Vector3(transition.transform.position.x,target_pos.y,
                transition.transform.position.z), ref velocity, smooth_time, max_trans_speed);

        }
        /* else
         {
             transition.transform.position = Vector3.SmoothDamp(transition.transform.position, start_pos, ref velocity, smooth_time, max_trans_speed);
         } */
    }
    // Update is called once per frame
    public void ShowMainMenu()
    {
        Transition_Down();
        Invoke("Transition_Up",2f);
    }
    private void Start()
    {
        start_pos = transition.transform.position;  
    }
}
