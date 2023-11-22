using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public DialogContainer dialog = new DialogContainer();
    public static DialogController Instance;

    //Inicializa el objeto en el script.
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}