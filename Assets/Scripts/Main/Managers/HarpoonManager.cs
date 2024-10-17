using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonManager : MonoBehaviour
{
    public static HarpoonManager instance;

    [Header("PARAMETERS")]
    [SerializeField] int maxNumberOfHarpoons; //Max number of harpoons

    [Header("REFERENCES IN PROYECT")]
    [SerializeField] private GameObject harpoon;
    [SerializeField] private GameObject fakeHarpoon;

    [Header("CHECKERS")]
    public int ammo; //Actual number of harpoons

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

        //Set the ammo as the maximum number of harpoons player is able to have
        ammo = maxNumberOfHarpoons;
    }

    private void FireHarpoon()
    {
        //Player have ammo
        if (ammo > 0)
        {
            ammo--;

            //Instantiate the harpoon
            GameObject harpoonShooted = Instantiate(harpoon, shootPoint.position, Quaternion.identity);

            //Assign a parent for order propouses
            harpoonShooted.transform.parent = GameObject.Find("*NC*_HarpoonParent").transform;

            //Change the name of the harpoon for debug settings
            harpoonShooted.name = $"harpoon{ammo}";
        }

        //Player dont have ammo
        else
        {
            Debug.Log("You dont have any ammo left");
        }       
    }

    private void OnDisable()
    {
        //Des-subscribe shoot event
        harpoonLauncher.ShootHarpoon -= FireHarpoon;
    }

    public void GenerateFakeHarpoon(Collision2D collision, HarpoonBehaviour destroyedHarpoonBehaviour, Transform spawnTransform, float bounceDirection)
    {
        GameObject fakeHarpoonInstance = Instantiate(fakeHarpoon, spawnTransform.position, Quaternion.identity);

        //Assign a parent for order propouses
        fakeHarpoonInstance.transform.parent = GameObject.Find("*NC*_FakeHarpoonParent").transform;
        fakeHarpoonInstance.GetComponent<FakeHarpoonBehaviour>().bounceDirection = bounceDirection;

        Destroy(collision.gameObject);
    }
}
