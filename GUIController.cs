using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GUI controller
// place on gameobject "GUI"

public class GUIController : MonoBehaviour
{
    public bool blink = true;
    public Text ScreenText;

    public GameObject Game;
    public gameController GameState;

    public GameObject currentCounter;

    void Start()
    {
        ScreenText = transform.GetChild(0).GetComponent<Text>();
        GameState = Game.GetComponent<gameController>();

        ShowText(
            "<color=red>[A]</color>: turn red car \n" +
            "<color=blue>[L]</color>: turn blue car \n" +
            "<color=yellow>[SPACE]</color>: start game"
        );
    }

    public void ShowText(string text)
    {
        ScreenText.text = text;
        blink = true; 
        StartCoroutine(BlinkText());
    }

    public IEnumerator BlinkText()
    {
        while (blink)
        {
            ScreenText.enabled = true;
            yield return new WaitForSeconds(.3f);
            ScreenText.enabled = false;
            yield return new WaitForSeconds(.3f);
        }

    }

    // update the lives counter, called from carController
    public void UpdateLivesCounter(string car, int lives)
    {
        // which of the two counters to adjust?
        if (car == "BlueCar")
        {
            currentCounter = GameObject.Find("BlueCarLives");
        } else
        {
            currentCounter = GameObject.Find("RedCarLives");
        }

        // first show all lives
        for (int i = 0; i < 3; i++)
        {
            currentCounter.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = true;
        }

        // then hide the correct amount of lives
        for (int i = 2; i > lives-1; i--)
        {
            currentCounter.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
        }

    }

    void Update ()
    {
        // listen for keystrokes to start gameplay while blinking text
        if (Input.GetKeyDown("space"))
        {
            // restarting game from winscreen? engage new game from carcontroller
            if (!GameState.playing && GameState.winScreen)
            {
                GameObject.Find("BlueCar").GetComponent<carController>().NewGame();
                GameState.playing = true;
                blink = false;
            }
            // starting game from scratch? keep loaded track, as in not start from carcontroller
            else if (!GameState.playing && !GameState.winScreen)
            {
                GameObject.Find("Camera").GetComponent<AudioSource>().Play();
                GameState.playing = true;
                blink = false;
            }

        }
    }
}
