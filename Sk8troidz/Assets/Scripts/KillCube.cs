using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player_Health ph = collision.gameObject.GetComponent<Player_Health>();
            if (ph != null)
            {
                ph.Remove_Health(1000);
            }
        }
    }
}
