using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Trick_System : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] HoldButton trick_btn;
    [SerializeField] HoldButton jump_btn;
    [SerializeField] Slider slider;
    [SerializeField] float speed;
    public int counter;
    [SerializeField] GameObject parent;
    [SerializeField] GameObject player;
    [SerializeField] GameObject super_bar;
    [SerializeField] float amount;
    [SerializeField] float delay; //delay after each successful trick
    private Weapon_Handler wh;
    [SerializeField] Animator animator;
    [SerializeField] GameObject crosshair;
    [SerializeField] Railgrinding railgrinding;
    Super_Bar sb;

    void Start()
    {

        wh = player.GetComponent<Weapon_Handler>();
        sb = player.GetComponent<Super_Bar>();
       // Cursor.lockState = CursorLockMode.Locked;
       // WebGLInput.stickyCursorLock = false;

    }
    
    public void Start_Trick_System()
    {
        counter = 0;
        PlayerMovement.trick_mode_activated = true;
        wh.weapon = null;
        crosshair.SetActive(false);
        animator.SetLayerWeight(2, 0);
        animator.SetBool("trickModeActivated", true);
        StartCoroutine(Trick(4));
    }

    public void Start_Rail_Trick_System()
    {
        PlayerMovement.trick_mode_activated = true;
        wh.weapon = null;
        crosshair.SetActive(false);
        animator.SetLayerWeight(2, 0f);
        animator.SetBool("trickModeActivated", true);
        animator.SetFloat("animSpeedCap", 0);
        StartCoroutine(RailTrick());
    }

    public void AddToCounter(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = false;
        btn.GetComponent<Image>().enabled = false;
        counter++;
    }
    IEnumerator Trick(int n)
    {

        while ((Input.GetButton("Fire2") || trick_btn.isDown) && PlayerMovement.trick_mode_activated == true && slider.value != slider.maxValue)
        {
            slider.value += Time.deltaTime * speed;
            yield return null;
        }
        // player.GetComponent<Animator>().enabled = false;


        animator.SetBool("trickModeActivated", false);
        if (slider.value == slider.maxValue)
        {
            sb.ChangeAmount(100);
            crosshair.SetActive(true);
        }
        else if (!(Input.GetButton("Fire2") || trick_btn.isDown))
        {
            crosshair.SetActive(true);
        }
        else
        {
            Ragdoll rd = player.GetComponent<Ragdoll>();
            rd.ActivateRagdolls();
        }

    }
    IEnumerator RailTrick()
    {
        while (((Input.GetButton("Fire2") || trick_btn.isDown) && railgrinding.progress >= 0 && railgrinding.progress <= 1) && railgrinding.onRail)
        {
            Debug.Log(animator.GetBool("trickModeActivated"));
            slider.value += Time.deltaTime * speed/200f;
            yield return null;
        }
        animator.SetBool("trickModeActivated", false);       
        
        if (slider.value == slider.maxValue)
        {
            sb.ChangeAmount(100);
            crosshair.SetActive(true);
        }
        else if (railgrinding.onRail)
        {
            yield return null;
        }
        else if (!(Input.GetButton("Fire2") || trick_btn.isDown) || (Input.GetButton("Jump") || jump_btn.isDown))
        {
            railgrinding.onRail = false;
            railgrinding.progress = 0;
            if(Input.GetButton("Jump") || jump_btn.isDown)
            {
             crosshair.SetActive(true);
            }
        }       
        else
        {
            railgrinding.progress = 0;
            railgrinding.onRail = false;
            Ragdoll rd = player.GetComponent<Ragdoll>();
            rd.ActivateRagdolls();
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
