using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Trick_System : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] HoldButton trick_btn;
    [SerializeField] HoldButton jump_btn;
    [SerializeField] Slider slider;
    [SerializeField] float speed; //speed the super_bar grows by
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

    [SerializeField] float total_multiplier_duration = 5f;
    [SerializeField] float multiplier_duration = 0;
    [SerializeField] float multiplier = 0;
    [SerializeField] List<GameObject> list;
    [SerializeField] Slider duration_slider;

    void Start()
    {

        wh = player.GetComponent<Weapon_Handler>();
        sb = player.GetComponent<Super_Bar>();
        Cursor.lockState = CursorLockMode.Locked;
        list = new List<GameObject>();

    }
    
    public void Start_Trick_System()
    {
        counter = 0;
        PlayerMovement.trick_mode_activated = true; //redundant code, could be moved to Trick() coroutine
        if (multiplier_duration <= 0) 
        {
            multiplier_duration = total_multiplier_duration;
            multiplier = 1;
            list.Clear();
            StartCoroutine(StartCountdown());
        }
       if (!list.Contains(PlayerMovement.last_collision))
        {
            list.Add(PlayerMovement.last_collision);
            multiplier++;
            multiplier_duration = total_multiplier_duration;
        }

        wh.weapon = null;
        crosshair.SetActive(false);
        animator.SetLayerWeight(2, 0);
        animator.SetBool("trickModeActivated", true);
        StartCoroutine(Trick(4));
    }

    public void Start_Rail_Trick_System()
    {

        PlayerMovement.trick_mode_activated = true;

        if (multiplier_duration <= 0)
        {
            multiplier_duration = total_multiplier_duration;
            multiplier = 1;
            list.Clear();
            StartCoroutine(StartCountdown());
        }
        if (!list.Contains(PlayerMovement.last_collision))
        {
            list.Add(PlayerMovement.last_collision);
            multiplier++;
            multiplier_duration = total_multiplier_duration;
        }
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
            slider.value += (Time.deltaTime * speed) * (multiplier);
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
            slider.value += (Time.deltaTime * speed / 200f) * (multiplier);
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
    private IEnumerator StartCountdown()
    {
        
        while (multiplier_duration > 0)
        {
            duration_slider.value = multiplier_duration;
            yield return new WaitForSeconds(0.2f);
            multiplier_duration -= 0.2f;
        }
        duration_slider.value = multiplier_duration;
        multiplier_duration = 0;
        multiplier = 0;
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
