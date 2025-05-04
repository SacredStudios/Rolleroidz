using UnityEngine;
using Photon.Pun;
using System.Diagnostics;

[CreateAssetMenu(fileName = "PoisonWeapon", menuName = "Scriptable Objects/PoisonWeapon")]
public class PoisonWeapon : RangedWeapon
{
    public float _poisonRadius = 5f;
    protected override float Radius => _poisonRadius;
    // PoisonWeapon.cs
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "PoisonWeapon", menuName = "Scriptable Objects/PoisonWeapon")]
public class PoisonWeapon : RangedWeapon
{
    [Header("Poison Settings")]
    [SerializeField] private float poisonDuration = 5f; // seconds

    protected override void ApplyDamage(RaycastHit hit, GameObject parent)
    {
        // (1) do all the normal damage/death logic
        base.ApplyDamage(hit, parent);

        // (2) spawn a poison VFX on the target
        if (particle_explosion != null)
        {
            PhotonNetwork.Instantiate(
                particle_explosion.name,
                hit.point,
                Quaternion.identity
            );
        }

        // (3) apply poison status over time if you have it on Player_Health:
        var ph = hit.collider.GetComponentInParent<Player_Health>();
        if (ph != null)
        {
            // ph.ApplyPoison(poisonDuration);
            Debug.Log("test");
        }
    }
}

}
