using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField, Tooltip("Ÿ—˜‚ÌTimeline")]
    private PlayableDirector _victoryTimeline;

    // Start is called before the first frame update
    void Start()
    {
        _victoryTimeline.Play();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
