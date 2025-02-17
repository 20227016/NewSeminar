using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private SessionManager _sessionManager = default;

    [SerializeField]
    private TMP_InputField _roomNameInputField = default;

    private void Awake()
    {
        _sessionManager = FindObjectOfType<SessionManager>();
    }

    public void Decision()
    {
        if (!string.IsNullOrWhiteSpace(_roomNameInputField.text))
        {
            _sessionManager.SessionName = _roomNameInputField.text;
            SceneManager.LoadScene("高橋デバック");
        }
        else
        {
            SceneManager.LoadScene("高橋デバック");
        }
    }
}
