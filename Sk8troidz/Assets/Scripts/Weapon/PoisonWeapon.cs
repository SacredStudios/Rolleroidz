using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "PoisonWeapon", menuName = "Scriptable Objects/PoisonWeapon")]
public class PoisonWeapon : RangedWeapon
{
    public float _poisonRadius = 5f;
    protected override float Radius => _poisonRadius;
    
    protected override void ApplyDamage(Player_Health ph, GameObject parent, RaycastHit hit)
    {
        // (1) do all the normal damage/death logic
        base.ApplyDamage(ph, parent, hit);

        // (2) spawn a poison VFX on the target
        if (particle_explosion != null)
        {
            PhotonNetwork.Instantiate(
                particle_explosion.name,
                hit.point,
                Quaternion.identity
            );
        }

        if (ph != null)
        {
            // ph.ApplyPoison(poisonDuration);
            Debug.Log("test");
        }
    }
}


