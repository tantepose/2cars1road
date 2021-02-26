using UnityEngine;
using System.Collections;

// car controller
// place on "RedCar" & "BlueCar" 

public class carController : MonoBehaviour
{
    public string turnKey;
    public bool isTurned;

    public float startSpeed;
    public float speedModifier;
    public float speed;
    public int lives = 3;

    public Rigidbody rigidBody;
    public bool crashed = false;
    public int crashForce = -50;

    public float pos1;
    public float pos2;
    public Vector3 startPosition; 

    public bool godMode;
    public bool winner;

    public GameObject game;
    public gameController gameState;

    public GameObject otherCar;
    
    public AudioSource CrashSound;
    public AudioSource EngineSound;
    public AudioSource SqueekSound; 

    private void Start ()
    {
        Debug.Log("*** GAME STARTED ***");

        // get data for turning
        pos1 = transform.position.x;
        pos2 = transform.position.x - 3;
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();

        // set values for speed and physics
        startSpeed = 15F;
        speedModifier = 0.02F;
        speed = startSpeed;
        crashForce = -50;

        // get gamestate object, stop smoke, set livescounter
        gameState = game.GetComponent<gameController>();
        gameObject.transform.Find("Smoke").GetComponent<ParticleSystem>().Stop();
        GameObject.Find("GUI").GetComponent<GUIController>().UpdateLivesCounter(gameObject.name, lives);
    }

    // updating forward movement
    private void FixedUpdate()
    {
        // game in progress? 
        if (gameState.playing)
        {
            // drive car forward if not crashed
            if (!crashed)
            {
                speed = speed + speedModifier;
                rigidBody.velocity = new Vector3(0, 0, speed);
                EngineSound.pitch = speed / startSpeed;
            }
        }
    }

    // updating keystrokes and turning
    private void Update()
    {

        // move only when specified key is pressed and not crashed
        if (Input.GetKeyDown(turnKey) && crashed == false)
        {
            isTurned = !isTurned;
            SqueekSound.pitch = Random.Range(1.0f, 1.5f);
            SqueekSound.Play();
        }

        // actually do the turn
        if (isTurned)
        {
            transform.position = new Vector3(pos2, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(pos1, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // crash if collision gameobject is not tagged "safe" & not crashed allready & not in godmode
        if (collision.gameObject.tag != "safe" && !crashed && !godMode)
        {
            Crash();
        }
    }

    private void Crash ()
    {
        // crash car
        crashed = true;
        rigidBody.AddForce(transform.forward * crashForce, ForceMode.Impulse); // apply extra stopping power
        transform.Find("Smoke").GetComponent<ParticleSystem>().Play();

        lives--;
        GameObject.Find("GUI").GetComponent<GUIController>().UpdateLivesCounter(gameObject.name, lives);

        //shake camera
        GameObject.Find("Camera").GetComponent<Animator>().Play("camerashake");

        // set soundscape
        CrashSound.Play();
        EngineSound.Stop();
        GameObject.Find("Camera").GetComponent<AudioSource>().Stop();
        
        // disable crashing for opponent
        otherCar.GetComponent<carController>().godMode = true;

        // end the game, or just end the round?
        if (lives <= 0)
        {
            EndGame();
        } else
        {
            StartCoroutine(EndRound());
        }

    }

    IEnumerator EndRound()
    {
        Time.timeScale = 0.6f;
        yield return new WaitForSeconds(1.5f);
        NewRound();
    }

    private void EndGame()
    {
        Debug.Log("*** END GAME ***");

        otherCar.GetComponent<carController>().winner = true; 
        otherCar.transform.Find("Crown").gameObject.GetComponent<Renderer>().enabled = true;

        otherCar.GetComponent<carController>().crashed = true;
        otherCar.GetComponent<carController>().EngineSound.Stop();
        otherCar.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        gameState.playing = false;
        gameState.winScreen = true;

        // blink text with right winner
        if (gameObject.name == "BlueCar")
        {
            GameObject.Find("GUI").GetComponent<GUIController>().ShowText(
                "<color=red>red</color> car wins \n" +
                "game over \n" +
                "<color=yellow>[SPACE]</color>: restart game"
            );

        } else
        {
            GameObject.Find("GUI").GetComponent<GUIController>().ShowText(
                "<color=blue>blue</color> car wins \n" +
                "game over \n" +
                "<color=green>[SPACE]</color>: restart game"
            );
        }
    }

    public void NewGame ()
    {
        Debug.Log("*** NEW GAME ***");

        lives = 3;
        otherCar.GetComponent<carController>().lives = 3;
        GameObject.Find("GUI").GetComponent<GUIController>().UpdateLivesCounter("BlueCar", 3);
        GameObject.Find("GUI").GetComponent<GUIController>().UpdateLivesCounter("RedCar", 3);

        gameObject.transform.Find("Crown").gameObject.GetComponent<Renderer>().enabled = false;
        otherCar.transform.Find("Crown").gameObject.GetComponent<Renderer>().enabled = false;

        GameObject.Find("Camera").GetComponent<AudioSource>().Play();

        NewRound();
    }

    private void NewRound()
    {
        Debug.Log("*** NEW ROUND ***");

        Time.timeScale = 1;

        game.GetComponent<gameController>().newTrack();
        ResetCars();

        GameObject.Find("Camera").GetComponent<AudioSource>().Play();
    }

    // reset both cars after crashes
    public void ResetCars()
    {
        Debug.Log("reset cars...");

        winner = false;
        otherCar.GetComponent<carController>().winner = false;

        crashed = false;
        otherCar.GetComponent<carController>().crashed = false;

        godMode = false;
        otherCar.GetComponent<carController>().godMode = false;

        EngineSound.Play();
        otherCar.GetComponent<carController>().EngineSound.Play();

        transform.Find("Smoke").GetComponent<ParticleSystem>().Stop();
        otherCar.transform.Find("Smoke").GetComponent<ParticleSystem>().Stop();

        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        otherCar.GetComponent<Rigidbody>().velocity = Vector3.zero;

        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        otherCar.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        gameObject.GetComponent<carController>().speed = startSpeed;
        otherCar.GetComponent<carController>().speed = startSpeed;

        gameObject.transform.position = startPosition;
        otherCar.transform.position = otherCar.GetComponent<carController>().startPosition;

        gameObject.transform.rotation = Quaternion.identity;
        otherCar.transform.rotation = Quaternion.identity;
    }

}
