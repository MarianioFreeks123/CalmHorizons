using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("CHECKERS")]
    public Vector2 playerPosition;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] Transform playerTransform;

    //Create the singleton
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }

    void Update()
    {
        GetPlayerPosition();
    }

    private void GetPlayerPosition()
    {
        playerPosition = playerTransform.position;
    }
}
