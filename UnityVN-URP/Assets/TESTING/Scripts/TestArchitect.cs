using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing {
    public class TestArchitect : MonoBehaviour
    {
        // Start is called before the first frame update
        DialogController dc;
        void Start()
        {
            dc = DialogController.DialogInstance;
            dc.DialogContainer;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

