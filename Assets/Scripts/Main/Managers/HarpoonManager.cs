using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonManager : MonoBehaviour
{
    public static HarpoonManager instance;

    [Header("REFERENCES IN PROYECT")]
    [SerializeField] private GameObject harpoon;

    [Header("CHECKERS")]
    [SerializeField] private List<GameObject> harpoons = new List<GameObject>();

    private int amountOfHarpoons;
    private Transform shootPoint; 
    private HarpoonLauncher harpoonLauncher;

    //Manage the singleton
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        else instance = this;
    }

    void Start()
    {
        //Assign References
        harpoonLauncher = GameObject.Find("*NC*_Player").GetComponent<HarpoonLauncher>();
        shootPoint = GameObject.Find("*NC*_ShootPoint").transform;

        //Subscribe shoot event
        harpoonLauncher.ShootHarpoon += FireHarpoon;
    }

    private void FireHarpoon()
    {
        //Get the oldest harpoon if there are five in scene and delete
        if (amountOfHarpoons == 5)
        {
            Destroy(harpoons[0]);
            harpoons.RemoveAt(0);
        } 

        GameObject harpoonShooted = Instantiate(harpoon, shootPoint.position, Quaternion.identity);
        harpoonShooted.transform.parent = GameObject.Find("*NC*_HarpoonParent").transform;
        harpoons.Add(harpoonShooted);
        amountOfHarpoons = harpoons.Count;
    }

    private void OnDisable()
    {
        //Des-subscribe shoot event
        harpoonLauncher.ShootHarpoon -= FireHarpoon;
    }
}
