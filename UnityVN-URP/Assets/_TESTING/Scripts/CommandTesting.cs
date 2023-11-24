using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CommandController.Instance.Execute("Printear");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
