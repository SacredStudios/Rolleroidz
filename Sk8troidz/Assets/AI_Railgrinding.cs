
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using Photon.Pun;
using UnityEngine.AI;

public class AI_Railgrinding : MonoBehaviour
{

    public bool onRail;
    [SerializeField] float speed;
    [SerializeField] float min_speed;

    [SerializeField] float height_offset;
    float time_for_spline;
    float elapsed_time;
    [SerializeField] float lerp_speed = 10f;
    public float progress;

    [SerializeField] Rail curr_rail; //script for rail
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] GameObject sparks;
    [SerializeField] PhotonView pv;
    public AI_Movement movement;
    [SerializeField] NavMeshAgent agent;
    bool dir;

    void FixedUpdate()
    {
        if (onRail == true)
        {
            MoveAlongRail();
        }
    }

    void MoveAlongRail()
    {

        if (curr_rail != null && onRail == true)
        {
            animator.SetFloat("animSpeedCap", 0);
            progress = elapsed_time / time_for_spline;
            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }

            float deltaTime = Time.deltaTime;


            float next_time_normalized;
            if (dir == true)
            {
                next_time_normalized = (elapsed_time + deltaTime) / time_for_spline;
            }
            else
            {
                next_time_normalized = (elapsed_time - deltaTime) / time_for_spline;
            }
            float3 pos, tangent, up;
            float3 next_pos_float, next_tan, next_up;
            SplineUtility.Evaluate(curr_rail.rail_spline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(curr_rail.rail_spline.Spline, next_time_normalized, out next_pos_float, out next_tan, out next_up);

            Vector3 world_pos = curr_rail.LocalToWorld(pos);
            Vector3 next_pos = curr_rail.LocalToWorld(next_pos_float);

            transform.position = world_pos + (transform.up * height_offset);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(next_pos - world_pos), lerp_speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerp_speed * Time.deltaTime);

            if (dir == true)
            {
                elapsed_time += deltaTime;
            }
            else
            {
                elapsed_time -= deltaTime;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rail" && !onRail && pv.IsMine)
        {
            animator.SetLayerWeight(3, 1f);
            animator.SetFloat("animSpeedCap", 0);
            onRail = true;
            curr_rail = collision.gameObject.GetComponent<Rail>();        
            SetRailPosition();
            rb.useGravity = false;
            speed = min_speed + Mathf.Abs(rb.linearVelocity.x + rb.linearVelocity.z);
           // sparks.SetActive(true);
        }
    }



    void SetRailPosition()
    {
        agent.enabled = false;
        Debug.Log("setting rail position");
        time_for_spline = curr_rail.length / speed;

        Vector3 spline_point;
        float normalized_time = curr_rail.CalcTargetRailPoint(transform.position, out spline_point);
        elapsed_time = time_for_spline * normalized_time;
        float3 pos, forward, up;
        SplineUtility.Evaluate(curr_rail.rail_spline.Spline, normalized_time, out pos, out forward, out up);
        curr_rail.CalcDirection(forward, transform.forward);
        transform.position = spline_point + (transform.up * height_offset);

    }
    public void ThrowOffRail()
    {


        //   sparks.SetActive(false);
        agent.enabled = true;
        animator.SetLayerWeight(3, 0f);
        onRail = false;
        curr_rail = null;
        transform.position += transform.forward * 1;
        rb.useGravity = true;
        movement.current_state = AI_Movement.State.Searching;
    }
    
}

