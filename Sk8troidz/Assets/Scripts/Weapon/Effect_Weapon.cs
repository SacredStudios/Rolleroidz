using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "Effect_Weapon", menuName = "Scripts/Effect_Weapon")]
public class Effect_Weapon : Weapon
{
public GameObject effect;
public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        GameObject clone = PhotonNetwork.Instantiate(effect.name, parent.transform.position, Quaternion.identity);
        clone.transform.parent = player.transform;
        clone.GetComponent<Black_Hole>().pv = this.pv;
        clone.GetComponent<Black_Hole>().player = this.player;
    }
    
}
