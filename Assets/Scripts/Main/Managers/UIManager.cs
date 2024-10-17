using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] HarpoonLauncher harpoonLauncher;
    [SerializeField] Animator[] harpoonImagesAnim;

    //Create singleton
    private void Awake()
    {
      if (instance != null && instance != this) Destroy(this);
      else instance = this;
    }

    void Start()
    {
        //Subscribe to shoot event
        harpoonLauncher.ShootHarpoon += ManageHarpoonUIImages;
    }

    private void ManageHarpoonUIImages()
    {
        //Check how many harpoons player have
        int ammo = HarpoonManager.instance.ammo;
        if (ammo >= 0) harpoonImagesAnim[ammo].SetBool("isShooted", true);
    }
}
