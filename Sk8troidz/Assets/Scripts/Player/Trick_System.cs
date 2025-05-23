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
    public float multiplier = 0.5f;
    [SerializeField] List<GameObject> list;
    [SerializeField] Slider duration_slider;
    private KeyCode trickKey;
    [SerializeField] Text trick_text;
    [SerializeField] float rayCastLength;
    [SerializeField] GameObject jump_pos;
    void Start()
    {
        wh = player.GetComponent<Weapon_Handler>();
        sb = player.GetComponent<Super_Bar>();      
        list = new List<GameObject>();
        trickKey = (KeyCode)PlayerPrefs.GetInt("TrickKey", (int)KeyCode.T);
    }
    
    
    public void Start_Trick_System()
    {
        counter = 0;
        PlayerMovement.trick_mode_activated = true; //redundant code, could be moved to Trick() coroutine
        if (multiplier_duration <= 0) 
        {
            multiplier_duration = total_multiplier_duration;
            multiplier = 0.5f;
            list.Clear();
            StartCoroutine(StartCountdown());
        }
       if (!list.Contains(PlayerMovement.last_collision))
        {
            list.Add(PlayerMovement.last_collision);
            Add_To_Multiplier();
        }

        wh.weapon = null;
        crosshair.SetActive(false);
        animator.SetLayerWeight(2, 0);
        animator.SetBool("trickModeActivated", true);
        StartCoroutine(Trick(4));
    }
    public void Add_To_Multiplier()
    {
        if (multiplier_duration <= 0)
        {
            multiplier_duration = total_multiplier_duration;
            StartCoroutine(StartCountdown());
        }
        multiplier += 0.5f;
        multiplier_duration = total_multiplier_duration;
        
    }

    public void Start_Rail_Trick_System()
    {

        PlayerMovement.trick_mode_activated = true;

        if (multiplier_duration <= 0)
        {
            multiplier_duration = total_multiplier_duration;
            multiplier = 0.5f;
            list.Clear();
            StartCoroutine(StartCountdown());
        }
        if (!list.Contains(PlayerMovement.last_collision))
        {
            list.Add(PlayerMovement.last_collision);
            Add_To_Multiplier();
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
        
        while ((Input.GetKey(trickKey) || trick_btn.isDown) && PlayerMovement.trick_mode_activated == true && slider.value != slider.maxValue)
        {
            slider.value += (Time.deltaTime * speed) * (multiplier + 0.5f);
            if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength))
            {
                trick_text.text = "LET GO";
                PlayerMovement.currentPrompt = PlayerMovement.TrickPrompt.LetGo;
            }
            yield return null;
        }
        // player.GetComponent<Animator>().enabled = false;

        animator.SetBool("trickModeActivated", false);
        if (slider.value == slider.maxValue)
        {
            sb.ChangeAmount(100);
            crosshair.SetActive(true);
        }
        else if (!(Input.GetKey(trickKey) || trick_btn.isDown))
        {
            crosshair.SetActive(true);
            PlayerMovement.currentPrompt = PlayerMovement.TrickPrompt.Succeeded;
            trick_text.text = "";
        }
        else
        {
            PlayerMovement.currentPrompt = PlayerMovement.TrickPrompt.Fail;
            trick_text.text = "<i>BUMMER</i>";
            Ragdoll rd = player.GetComponent<Ragdoll>();
            rd.ActivateRagdolls();
        }

    }
    IEnumerator RailTrick()
    {
        while (((Input.GetKey(trickKey) || trick_btn.isDown) && railgrinding.progress >= 0 && railgrinding.progress <= 1) && railgrinding.onRail)
        {
            slider.value += (Time.deltaTime * speed / 200f) * (2f); //rail tricking doesnt have multiplier
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
        else if (!(Input.GetKey(trickKey) || trick_btn.isDown) || (Input.GetButton("Jump") || jump_btn.isDown))
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
        multiplier = 0.5f;
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
