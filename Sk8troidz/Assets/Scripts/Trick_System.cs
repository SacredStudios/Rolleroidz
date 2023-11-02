using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Trick_System : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject btn;
    public int counter;
    [SerializeField] GameObject parent;
    [SerializeField] GameObject player;
    [SerializeField] GameObject super_bar;
    [SerializeField] float amount;
    [SerializeField] float delay; //delay after each successful trick
    private Weapon_Handler wh;
    [SerializeField] Animator animator;
    
    Super_Bar sb;

    void Start()
    {

        wh = player.GetComponent<Weapon_Handler>();
        sb = player.GetComponent<Super_Bar>();

    }
    
    public void Start_Trick_System()
    {
        counter = 0;
        PlayerMovement.trick_mode_activated = true;
        StartCoroutine(Trick(4));
    }

    public void AddToCounter(GameObject btn)
    {
        animator.SetLayerWeight(2, 0);
        animator.SetBool("trickModeActivated", true);
        wh.weapon = null;
        counter++;
        Destroy(btn);
    }
    IEnumerator Trick(int n)
    {
        Debug.Log(n);
        counter = 0;
        Vector3 position = new Vector3(Screen.width/2, Screen.height / 2, 0f);
        for(int i = 0; i < n/2; i++)
        {
            position.y = Screen.height / 2 + Random.Range(-50, 50);
            GameObject btn_clone = Instantiate(btn, position, Quaternion.identity, parent.transform);
            btn_clone.SetActive(true);
            position.x += 90f;
        }
        position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        for (int i = 0; i < n / 2-1; i++)
        {
            position.y = Screen.height / 2 + Random.Range(-50, 50);
            position.x -= 90f;
            GameObject btn_clone = Instantiate(btn, position, Quaternion.identity, parent.transform);
            btn_clone.SetActive(true);
        }
        yield return new WaitUntil(() => counter >= n-1 || PlayerMovement.trick_mode_activated == false);
        // player.GetComponent<Animator>().enabled = false;


        animator.SetBool("trickModeActivated", false);
 
        if (counter >= n-1)
        {
            
            sb.ChangeAmount(amount);
            yield return new WaitForSeconds(delay);
            if (n > 8)
            {
                n = 8;
            }
                yield return Trick(4);
        }
        else if (counter>= 1)
        {

            Ragdoll rd = player.GetComponent<Ragdoll>();
            rd.ActivateRagdolls();
        }
        counter = 0;


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
