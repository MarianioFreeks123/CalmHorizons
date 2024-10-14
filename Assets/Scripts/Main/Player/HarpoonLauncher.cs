using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonLauncher : MonoBehaviour
{
    public delegate void HarpoonLauncherDelegate();
    public event HarpoonLauncherDelegate ShootHarpoon;

    [Header("PARAMETERS")]
    [SerializeField] private float shootCadence;    

    private bool canShoot = true;

    void Update()
    {
        //Shoot when player touches input and cadence time is complete
        if (canShoot && Input.GetButtonDown("Fire1")) StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        canShoot = false;

        //Shoot event if any object is subscribed to the event
        ShootHarpoon?.Invoke();

        yield return new WaitForSeconds(shootCadence);
        canShoot = true;
    }
}
