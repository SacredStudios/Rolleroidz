using UnityEngine;
using UnityEngine.UI;
public class Remap_Keys : MonoBehaviour
{
    [SerializeField] private InputField leftInput;
    [SerializeField] private InputField rightInput;
    [SerializeField] private InputField upInput;
    [SerializeField] private InputField downInput;

    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;

    private const string LeftKeyPref = "LeftKey";
    private const string RightKeyPref = "RightKey";
    private const string UpKeyPref = "UpKey";
    private const string DownKeyPref = "DownKey";

    void Start()
    {
        // Load keys from PlayerPrefs or set defaults
        leftKey = (KeyCode)PlayerPrefs.GetInt(LeftKeyPref, (int)KeyCode.A);
        rightKey = (KeyCode)PlayerPrefs.GetInt(RightKeyPref, (int)KeyCode.D);
        upKey = (KeyCode)PlayerPrefs.GetInt(UpKeyPref, (int)KeyCode.W);
        downKey = (KeyCode)PlayerPrefs.GetInt(DownKeyPref, (int)KeyCode.S);

        // Set InputFields to current key values
        leftInput.text = string.IsNullOrEmpty(leftKey.ToString()) ? KeyCode.A.ToString() : leftKey.ToString();
        rightInput.text = string.IsNullOrEmpty(rightKey.ToString()) ? KeyCode.D.ToString() : rightKey.ToString();
        upInput.text = string.IsNullOrEmpty(upKey.ToString()) ? KeyCode.W.ToString() : upKey.ToString();
        downInput.text = string.IsNullOrEmpty(downKey.ToString()) ? KeyCode.S.ToString() : downKey.ToString();


        // Add listeners for InputFields
        leftInput.onEndEdit.AddListener(delegate { UpdateKey(LeftKeyPref, ref leftKey, leftInput.text); });
        rightInput.onEndEdit.AddListener(delegate { UpdateKey(RightKeyPref, ref rightKey, rightInput.text); });
        upInput.onEndEdit.AddListener(delegate { UpdateKey(UpKeyPref, ref upKey, upInput.text); });
        downInput.onEndEdit.AddListener(delegate { UpdateKey(DownKeyPref, ref downKey, downInput.text); });
    }

    private void UpdateKey(string prefKey, ref KeyCode keyToUpdate, string newKey)
    {
        if (System.Enum.TryParse(newKey, true, out KeyCode parsedKey))
        {
            keyToUpdate = parsedKey;
            PlayerPrefs.SetInt(prefKey, (int)keyToUpdate); // Save to PlayerPrefs
            PlayerPrefs.Save(); // Persist changes
            Debug.Log($"{prefKey} updated to {keyToUpdate}");
        }
        else
        {
            Debug.LogWarning("Invalid key entered!");
        }
    }

    void Update()
    {
        // Example: Detect remapped keys during gameplay
        if (Input.GetKeyDown(leftKey))
        {
            Debug.Log("Left key pressed!");
        }
        if (Input.GetKeyDown(rightKey))
        {
            Debug.Log("Right key pressed!");
        }
        if (Input.GetKeyDown(upKey))
        {
            Debug.Log("Up key pressed!");
        }
        if (Input.GetKeyDown(downKey))
        {
            Debug.Log("Down key pressed!");
        }
    }
}
