using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("CharacterSelection");
    }
}
