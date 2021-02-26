using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

// camera controller
// place on gameobject "Camera"

public class cameraController : MonoBehaviour
{
    public GameObject RedCar;
    public GameObject BlueCar;
    public carController RedCarState;
    public carController BlueCarState; 

    public float cameraDisplace;

    public AudioSource Music;
    public bool playMusic;

    private void Start()
    {
        // calculate displacement to use in update
        cameraDisplace = RedCar.transform.position.z - transform.position.z;
        Music = GetComponent<AudioSource>();
        Music.Stop();

        // get cars' state from their carcontrollers
        RedCarState = RedCar.GetComponent<carController>();
        BlueCarState = BlueCar.GetComponent<carController>();
    }

    private void Update()
    {
        // follow bluecar, or the car who has crashed, or the winner
        if (RedCarState.crashed && !BlueCarState.winner)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, RedCar.transform.position.z - cameraDisplace);
        }

        else if (BlueCarState.crashed && !RedCarState.winner)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BlueCar.transform.position.z - cameraDisplace);
        }

        else if (RedCarState.winner)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, RedCar.transform.position.z - cameraDisplace);
        }

        else if (BlueCarState.winner)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BlueCar.transform.position.z - cameraDisplace);
        }

        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BlueCar.transform.position.z - cameraDisplace);
        }

        // music control by player and/or editor boolean
        if (Input.GetKeyUp("m"))
        {
            playMusic = !playMusic;
            if (playMusic)
            {
                Music.enabled = true;
            }
            else
            {
                Music.enabled = false;
            }
        }

        // graphic options
        if (Input.GetKeyUp("1"))
        {
            GetComponent<PostProcessLayer>().enabled = true;
            GetComponent<PostProcessVolume>().enabled = true;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.Soft;
        }

        if (Input.GetKeyUp("2"))
        {
            GetComponent<PostProcessLayer>().enabled = true;
            GetComponent<PostProcessVolume>().enabled = true;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.Hard;
        }

        if (Input.GetKeyUp("3"))
        {
            GetComponent<PostProcessLayer>().enabled = true;
            GetComponent<PostProcessVolume>().enabled = true;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.None;
        }

        if (Input.GetKeyUp("4"))
        {
            GetComponent<PostProcessLayer>().enabled = false;
            GetComponent<PostProcessVolume>().enabled = false;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.Soft;
        }

        if (Input.GetKeyUp("5"))
        {
            GetComponent<PostProcessLayer>().enabled = false;
            GetComponent<PostProcessVolume>().enabled = false;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.Hard;
        }

        if (Input.GetKeyUp("6"))
        {
            GetComponent<PostProcessLayer>().enabled = false;
            GetComponent<PostProcessVolume>().enabled = false;
            GameObject.Find("Light").GetComponent<Light>().shadows = LightShadows.None;
        }

    }
}
