using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);

        else instance = this;

    }
    public void DebugErrorController(string errorMessage)
    {
        string gameObjectName = this.transform.name;
        Debug.LogError(errorMessage + " in " + gameObjectName + " component is not found");
        Time.timeScale = 0.0f;
    }
}
