using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject gun;
    [SerializeField] GameObject body;
    [SerializeField] GameObject laser_loc;
    public GameObject particle_trail1;
    public GameObject particle_trail2;
    public GameObject impact_effect;

    void Update()
    {
      //  Shoot();
    }
    void Start()
    {
        if (particle_trail1 != null)
        {
            GameObject p_trail = Instantiate(particle_trail1, laser_loc.transform);
            p_trail.GetComponent<ParticleSystem>().Play();
        }
   
    }
    public void Shoot()
    {
        if (particle_trail1 != null)
        {
            GameObject p_trail = Instantiate(particle_trail1, laser_loc.transform);
            p_trail.GetComponent<ParticleSystem>().Play();
        }
        if (particle_trail2 != null)
        {
            GameObject p_trail = Instantiate(particle_trail2, laser_loc.transform);
            p_trail.GetComponent<ParticleSystem>().Play();
        }


        Debug.DrawRay(gun.transform.position, body.transform.forward*50, Color.green);
        Ray ray = new Ray(gun.transform.position, body.transform.forward*50);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit)) //Target Acquired
        {
           Debug.Log(hit.collider.name);
        }
        else
        {
            Debug.Log("");
        }

    }

}
