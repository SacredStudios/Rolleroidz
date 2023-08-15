using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class Trick_System : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject btn;
    public int counter;
    [SerializeField] GameObject parent;
    [SerializeField] GameObject Weapon_Handler;
    private Weapon_Handler wh;
    [SerializeField] PhotonView pv;
    void Start()
    {

        wh = Weapon_Handler.GetComponent<Weapon_Handler>();
    }
    public void Start_Trick_System()
    {
        counter = 0;
        PlayerMovement.trick_mode_activated = true;
        StartCoroutine(Trick(3));
    }

    public void AddToCounter(GameObject btn)
    {
        counter++;
        Destroy(btn);
    }
    IEnumerator Trick(int n)
    {
        Vector3 position = new Vector3(0f, 0f, 0f);
        for(int i = 0; i < n; i++)
        {
            position.x += 25f;
            GameObject btn_clone = Instantiate(btn, position, Quaternion.identity, parent.transform);
            btn_clone.SetActive(true);
        }
        yield return new WaitUntil(() => counter >= n || PlayerMovement.trick_mode_activated == false);
        if(counter >= n)
        {
            pv.Owner.AddScore(1);
        }
   
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        wh.isOverTrickBtn = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        wh.isOverTrickBtn = false;

    }
}
