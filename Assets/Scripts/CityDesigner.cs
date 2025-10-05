using System.Collections.Generic;
using UnityEngine;

public class CityDesigner : MonoBehaviour
{

    public CityDesigner Instance { get; private set; }

    public int density;

    public List<GameObject> cityBlocks;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }



}