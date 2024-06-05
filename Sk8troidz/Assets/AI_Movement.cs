using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    public Transform Target;
    public float update_speed = 5f;
    public NavMeshAgent agent;
    List<GameObject> playerList;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] GameObject jump_pos;
    [SerializeField] float rayCastLength;
    float currSpeed = 0;
    public enum State
    {
        Searching, Railgrinding, Tricking, Boosting
    }
    State current_state;

    Vector3 lastPos;
    void Start()
    {
        current_state = State.Searching;
        StartCoroutine(Check_For_Players());
        agent = GetComponent<NavMeshAgent>();
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Rail") //change this to a better tag name
        {
            agent.enabled = false;
        }        
    }
    private void Update()
    {
        if (current_state == State.Searching)
        {
            if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength))
            {
                animator.SetFloat("IsJumping", 0f);
                //offground_sound.Play();
            }
            else
            {
                animator.SetFloat("IsJumping", 1f);
            }
            currSpeed = (transform.position - lastPos).magnitude;
            lastPos = transform.position;
            if (currSpeed < 0.1)
            {
                animator.SetFloat("animSpeedCap", 0f);
            }
            else
            {
                animator.SetFloat("animSpeedCap", 1f);
            }
        }
    }

    IEnumerator Check_For_Players()
    {
        playerList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        yield return new WaitForSeconds(update_speed);
        StartCoroutine(Check_For_Players());
        if (playerList.Count > 0)
        {
            StartCoroutine(Follow_Target());
        }
    }
    IEnumerator Follow_Target()
    {
        List<GameObject> playerList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        if (playerList.Count == 0)
        {
            yield return new WaitForSeconds(update_speed);
            yield return Follow_Target();
        }
        Target = playerList[0].transform;
        yield return new WaitUntil(() => Target != null && agent != null);
        WaitForSeconds wait = new WaitForSeconds(update_speed);
        
         while(enabled)
         {           
            yield return wait;
            agent.SetDestination(Target.transform.position);

        }
        
        yield return wait;
    }
    
}
