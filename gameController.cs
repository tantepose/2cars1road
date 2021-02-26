using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// game controller for drawing random track
// place on gameobject "Game"

public class gameController : MonoBehaviour
{
    public GameObject theCamera;

    public GameObject[] sections;
    public GameObject nextSection;
    public float nextSectionPosition;
    public GameObject newSection;

    public int nextSectionID;
    public int lastSectionID;

    public bool playing = false;
    public bool winScreen = false; 

    private void Start()
    {
        newTrack();
    }

    // place new section in front of current one, when camera z position is... right
    private void Update()
    {
        if (theCamera.transform.position.z > (nextSectionPosition - newSection.transform.Find("Grass").gameObject.transform.localScale.z * 1.5))
        {
            PlaceNewSection();
        }
    }

    // refactor this with function variables instead of global variables
    private void PlaceNewSection()
    {
        // get random section from array
        nextSection = sections[GetRandomSectionID()];

        // place section at right z position with no rotation
        newSection = Instantiate(nextSection, new Vector3(0, 0, nextSectionPosition), Quaternion.identity);

        // place new section in game container, for easy removing later
        newSection.transform.parent = transform;

        // get z position of next section, based on length of current section
        nextSectionPosition = nextSectionPosition + nextSection.transform.Find("Grass").gameObject.transform.localScale.z;

        RemoveOldSection();
    }
    
    // get random section ID, but NOT same as the last one
    private int GetRandomSectionID ()
    {
        lastSectionID = nextSectionID; // store current sectionID, to compare with the new one
        nextSectionID = Random.Range(0, sections.Length);

        while (nextSectionID == lastSectionID) // try until sucsess
        {
            nextSectionID = Random.Range(0, sections.Length);
        }
        
        return nextSectionID;
    }

    private void RemoveOldSection()
    {
        // destroy previous section (first child) if more than two are present
        if (this.gameObject.transform.childCount > 2)
        {
            Destroy(this.gameObject.transform.GetChild(0).gameObject);
        }
    }

    // start new track
    public void newTrack()
    {
        // if sections allready exists, destroy them
        if (this.gameObject.transform.childCount > 0)
        {
            Destroy(this.gameObject.transform.GetChild(0).gameObject);
        }

        if (this.gameObject.transform.childCount > 1)
        {
            Destroy(this.gameObject.transform.GetChild(0).gameObject);
            Destroy(this.gameObject.transform.GetChild(1).gameObject);
        }

        // placing the very first section, at 0, 0, grass.length / 2 (default 60)
        nextSection = sections[GetRandomSectionID()];        
        nextSectionPosition = nextSection.transform.Find("Grass").gameObject.transform.localScale.z / 2;
        newSection = Instantiate(nextSection, new Vector3(0, 0, nextSectionPosition), Quaternion.identity);
        newSection.transform.parent = transform;
        nextSectionPosition = nextSectionPosition + nextSection.transform.Find("Grass").gameObject.transform.localScale.z;
    }
}
