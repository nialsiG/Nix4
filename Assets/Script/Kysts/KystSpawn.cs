using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KystSpawn : MonoBehaviour
{
    public SOKyst kyst;

    // Start is called before the first frame update
    void Start()
    {
        kyst.SpawnKyst(transform);
    }
}
