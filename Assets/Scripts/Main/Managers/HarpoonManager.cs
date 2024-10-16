using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonManager : MonoBehaviour
{
    public static HarpoonManager instance;

    [Header("PARAMETERS")]
    [SerializeField] int maxNumberOfHarpoons;

    [Header("REFERENCES IN PROYECT")]
    [SerializeField] private GameObject harpoon;
    [SerializeField] private GameObject fakeHarpoon;

    [Header("CHECKERS")]
    [SerializeField] private List<GameObject> harpoons = new List<GameObject>(); //Need to start with 
    public int nextHarpoonForShoot;

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

        //Assign harpoons empty spaces to the list size
        for (int i = 0; i < maxNumberOfHarpoons; i++)
        {
            harpoons.Add(null);
        }
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
        //Reset the counter when the maximun number of harpoons is reached
        if (nextHarpoonForShoot > maxNumberOfHarpoons - 1) nextHarpoonForShoot = 0;
        
        //Instantiate the harpoon
        GameObject harpoonShooted = Instantiate(harpoon, shootPoint.position, Quaternion.identity);

        //Assign a parent for order propouses
        harpoonShooted.transform.parent = GameObject.Find("*NC*_HarpoonParent").transform;

        if (harpoons[nextHarpoonForShoot] != null) Destroy(harpoons[nextHarpoonForShoot]);
        harpoons[nextHarpoonForShoot] = harpoonShooted;

        //Change the name of the harpoon for debug settings
        harpoonShooted.name = $"harpoon{nextHarpoonForShoot}";

        //Update the harpoon index
        nextHarpoonForShoot++;
    }

    private void OnDisable()
    {
        //Des-subscribe shoot event
        harpoonLauncher.ShootHarpoon -= FireHarpoon;
    }

    public void DestroyHarpoon(int harpoonToRemove)
    {
        harpoons.RemoveAt(harpoonToRemove);

        //Update the index
        nextHarpoonForShoot--;

        harpoons.Add(null);
    }

    public void GenerateFakeHarpoon(Collision2D collision, HarpoonBehaviour destroyedHarpoonBehaviour, Transform spawnTransform)
    {
        GameObject fakeHarpoonInstance = Instantiate(fakeHarpoon, spawnTransform.position, Quaternion.identity);

        //Assign a parent for order propouses
        fakeHarpoonInstance.transform.parent = GameObject.Find("*NC*_FakeHarpoonParent").transform;

        Destroy(collision.gameObject);
        DestroyHarpoon(destroyedHarpoonBehaviour.harpoonIndexInTheManager);
    }
}
