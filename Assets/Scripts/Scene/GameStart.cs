using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private SessionManager _sessionManager = default;

    [SerializeField]
    private TMP_InputField _roomNameInputField = default;

    [SerializeField]
    private TextMeshProUGUI _warningText = default;

    private void Awake()
    {
        _sessionManager = FindObjectOfType<SessionManager>();
        Cursor.lockState = CursorLockMode.None;
        _warningText.gameObject.SetActive(false);
    }

    public void Decision()
    {
        if (!string.IsNullOrWhiteSpace(_roomNameInputField.text))
        {
            _sessionManager.SessionName = _roomNameInputField.text;
            SceneManager.LoadScene("GameMain");
        }
        else
        {
            _warningText.text = "ƒ‹[ƒ€–¼‚ğ“ü—Í‚µ‚Ä‚­‚¾‚³‚¢";
            _warningText.gameObject.SetActive(true);
            //SceneManager.LoadScene("GameMain");
        }
    }
}
