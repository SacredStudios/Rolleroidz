using UnityEngine;
using UnityEngine.UI;

public class Remap_Keys : MonoBehaviour
{
    [SerializeField] private InputField leftInput;
    [SerializeField] private InputField rightInput;
    [SerializeField] private InputField upInput;
    [SerializeField] private InputField downInput;
    [SerializeField] private InputField jumpInput;
    [SerializeField] private InputField shootInput;
    [SerializeField] private InputField trickInput;

    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode jumpKey;
    private KeyCode shootKey;
    private KeyCode trickKey;

    private const string LeftKeyPref = "LeftKey";
    private const string RightKeyPref = "RightKey";
    private const string UpKeyPref = "UpKey";
    private const string DownKeyPref = "DownKey";
    private const string JumpKeyPref = "JumpKey";
    private const string ShootKeyPref = "ShootKey";
    private const string TrickKeyPref = "TrickKey";

    void Start()
    {
        // Load keys from PlayerPrefs or set defaults
        leftKey = (KeyCode)PlayerPrefs.GetInt(LeftKeyPref, (int)KeyCode.A);
        rightKey = (KeyCode)PlayerPrefs.GetInt(RightKeyPref, (int)KeyCode.D);
        upKey = (KeyCode)PlayerPrefs.GetInt(UpKeyPref, (int)KeyCode.W);
        downKey = (KeyCode)PlayerPrefs.GetInt(DownKeyPref, (int)KeyCode.S);
        jumpKey = (KeyCode)PlayerPrefs.GetInt(JumpKeyPref, (int)KeyCode.Space);
        shootKey = (KeyCode)PlayerPrefs.GetInt(ShootKeyPref, (int)KeyCode.Q);
        trickKey = (KeyCode)PlayerPrefs.GetInt(TrickKeyPref, (int)KeyCode.T);

        // Set InputFields to current key values or defaults
        leftInput.text = leftKey.ToString();
        rightInput.text = rightKey.ToString();
        upInput.text = upKey.ToString();
        downInput.text = downKey.ToString();
        jumpInput.text = jumpKey.ToString();
        shootInput.text = shootKey.ToString();
        trickInput.text = trickKey.ToString();

        // Add listeners for InputFields
        leftInput.onValueChanged.AddListener(delegate { UpdateKey(leftInput, LeftKeyPref, ref leftKey, leftInput.text); });
        rightInput.onValueChanged.AddListener(delegate { UpdateKey(rightInput, RightKeyPref, ref rightKey, rightInput.text); });
        upInput.onValueChanged.AddListener(delegate { UpdateKey(upInput, UpKeyPref, ref upKey, upInput.text); });
        downInput.onValueChanged.AddListener(delegate { UpdateKey(downInput, DownKeyPref, ref downKey, downInput.text); });
        jumpInput.onValueChanged.AddListener(delegate { UpdateKey(jumpInput, JumpKeyPref, ref jumpKey, jumpInput.text); });
        shootInput.onValueChanged.AddListener(delegate { UpdateKey(shootInput, ShootKeyPref, ref shootKey, shootInput.text); });
        trickInput.onValueChanged.AddListener(delegate { UpdateKey(trickInput, TrickKeyPref, ref trickKey, trickInput.text); });
        //Setting input field as "Space" if pref is set to space bar/
        //Otherwise this function is not needed as the values are set automatically on load
        SetInputFieldText(leftInput, leftKey);
        SetInputFieldText(rightInput, rightKey);
        SetInputFieldText(upInput, upKey);
        SetInputFieldText(downInput, downKey);
        SetInputFieldText(jumpInput, jumpKey);
        SetInputFieldText(shootInput, shootKey);
        SetInputFieldText(trickInput, trickKey);

    }

    private void UpdateKey(InputField inputField, string prefKey, ref KeyCode keyToUpdate, string newKey)
    {
       
        // Check if the user entered a single space
        if (inputField.text == " ")

        {
            Debug.Log("spacing out");
            inputField.characterLimit = 5; // Set the character limit to 5 for "Space"
            keyToUpdate = KeyCode.Space;
            inputField.text = "Space";
            PlayerPrefs.SetInt(prefKey, (int)keyToUpdate);
            PlayerPrefs.Save();
            Debug.Log($"{prefKey} updated to Space");
        }
        else {
            inputField.characterLimit = 1;
            if (inputField.text.Length < 5 && inputField.text.Length > 1) 
            {
              inputField.text = "";
            }
            if (System.Enum.TryParse(newKey, true, out KeyCode parsedKey))
            {
                keyToUpdate = parsedKey;
                PlayerPrefs.SetInt(prefKey, (int)keyToUpdate);
                PlayerPrefs.Save();
            }
            else
            {
                // Set to default if invalid
                KeyCode defaultKey = GetDefaultKey(prefKey);
                keyToUpdate = defaultKey;
                PlayerPrefs.SetInt(prefKey, (int)defaultKey);
                PlayerPrefs.Save();
            }
        }
    }

    private KeyCode GetDefaultKey(string prefKey)
    {
        return prefKey switch
        {
            "LeftKey" => KeyCode.A,
            "RightKey" => KeyCode.D,
            "UpKey" => KeyCode.W,
            "DownKey" => KeyCode.S,
            "JumpKey" => KeyCode.Space,
            "ShootKey" => KeyCode.Q,
            "TrickKey" => KeyCode.T,
            _ => KeyCode.None // Default for unknown actions
        };
    }

    private void SetInputFieldText(InputField inputField, KeyCode keyCode)
    {
        if (keyCode == KeyCode.Space)
        {
            
            inputField.characterLimit = 5; // Adjust character limit for "Space"
            inputField.text = "Space";
        }
        else
        {
            inputField.text = keyCode.ToString();
            inputField.characterLimit = 1; // Default character limit for other keys
        }
    }

}
