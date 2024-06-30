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
    [SerializeField] GameObject laser_loc;
    float currSpeed = 0;
    public enum State
    {
        Searching, Railgrinding, Tricking, Boosting
    }
    public State current_state;

    Vector3 lastPos;
    void Start()
    {
        current_state = State.Searching;
        StartCoroutine(Check_For_Players());
        StartCoroutine(BendAndRotate());
        agent = GetComponent<NavMeshAgent>();
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Rail") //change this to a better tag name
        {
            current_state = State.Railgrinding;
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
            if (Target != null)
            {
                if (agent.remainingDistance <= 10)
                {
                    Vector3 targetDirection = Target.transform.position - transform.position;
                    targetDirection.y = 0;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                }
            }
        }
    }
    IEnumerator BendAndRotate()
    {
        while (enabled)
        {
            if (Target != null)
            {
                // Calculate the ray from the current position in the direction of 'up' multiplied by a distance.
                Ray ray = new Ray(laser_loc.transform.position, laser_loc.transform.up * 500f);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    float curr_bend = animator.GetFloat("Bend");

                    // Bending the GameObject based on the target's position relative to the hit point.
                    if (hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "Player_Head")
                    {
                        if (Target.transform.position.y > hit.point.y)
                        {
                            animator.SetFloat("Bend", curr_bend -= 20f);
                        }
                        if (Target.transform.position.y < hit.point.y)
                        {
                            animator.SetFloat("Bend", curr_bend += 20f);
                        }
                        if (curr_bend < -30) animator.SetFloat("Bend", -30);
                        if (curr_bend > 70) animator.SetFloat("Bend", 70);
                    }
                  
                }

                yield return new WaitForSeconds(0.01f);
            }
            yield return null;
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
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(Target.transform.position);
                
            }
            
        }
        
        yield return wait;
    }
    
}
