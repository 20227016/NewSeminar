using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField, Tooltip("èüóòéûÇÃTimeline")]
    private PlayableDirector _victoryTimeline;

    private float _victoryTime = default;

    // Start is called before the first frame update
    void Start()
    {
        _victoryTimeline.Play();
        _victoryTime = 3f;
    }

    private void Update()
    {
        _victoryTime -= Time.deltaTime;

        if (Input.anyKey && _victoryTime <= 0f)
        {
            _victoryTime = 3f;
            SceneManager.LoadScene("Title");
        }
    }
}
