using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "PoisonWeapon", menuName = "Scriptable Objects/PoisonWeapon")]
public class PoisonWeapon : RangedWeapon
{
    public float _poisonRadius = 5f;
    protected override float Radius => _poisonRadius;
    public GameObject poison_effect;
    public override void Shoot(GameObject parent, GameObject particlePos, GameObject explosionPos)
    {
        base.Shoot(parent, particlePos, explosionPos);
        PhotonNetwork.Instantiate(poison_effect.name, explosionPos.transform.position, explosionPos.transform.rotation);
    }
}
