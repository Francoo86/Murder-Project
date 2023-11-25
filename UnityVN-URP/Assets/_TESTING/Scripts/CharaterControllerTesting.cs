using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterControllerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Character character = CharacterController.Instance.CreateCharacter("Alfonso");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
