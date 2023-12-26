using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using UnityEditor;
using System.IO;
using VISUALNOVEL;

public class TestingDialog : MonoBehaviour
{

    [SerializeField] private TextAsset fileToRead = null;
    private SpriteCharacter charPedro;
    private SpriteCharacter charPatricia;
    
    // Start is called before the first frame update
    void Start()
    {
        charPedro = CharacterController.Instance.CreateCharacter("Marcelo") as SpriteCharacter;
        charPatricia = CharacterController.Instance.CreateCharacter("Patricia") as SpriteCharacter;

        charPatricia.Show();
        charPedro.Show();
        //StartCoroutine(StartTalking());
    }

    IEnumerator StartTalking()
    {
        string fullPath = AssetDatabase.GetAssetPath(fileToRead);
        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10);
        string filePath = Path.ChangeExtension(relativePath, null);

        LoadFile(filePath);

        SpriteCharacter charPedro = CharacterController.Instance.CreateCharacter("Marcelo") as SpriteCharacter;
        Character alfonso = CharacterController.Instance.CreateCharacter("Patricia");
        List<string> lines = new List<string>()
        {
            "Podría poner un Lorem Ipsum aquí, pero siento que esto podría ser más interactivo.",
            "No se, probablemente esta sea una linea generica numero 2.",
            "Testeando linea 3",
            "Testeando linea 4",
        };

        AudioController.Instance.PlayTrack("Audio/Music/SneakySnitch", startVol: 0.7f);
        charPedro.SetDialogColor(Color.red);

        yield return new WaitForSeconds(1f);
        yield return charPedro.Hide();
        yield return new WaitForSeconds(1f);
        yield return charPedro.Show();

        //TODO: Usar el patron introduce parameter object en este.
        AudioController.Instance.PlayTrack("Audio/Music/Comedy", 0, startVol: 0.5f);
        AudioController.Instance.PlayTrack("Audio/Music/SneakySnitch", 1, startVol: 0.7f);

        yield return new WaitForSeconds(1f);
        Sprite pensativo = charPedro.GetSprite("feliz");
        Debug.Log("Transitioning to pensativo");
        charPedro.TransitionSprite(pensativo);
        //charPedro.SetSprite(pensativo);
        //Wacky ahh pos.
      

        //AudioController.Instance.PlaySoundEffect("Audio/SFX/RadioStatic");

        yield return charPedro.Say(lines);
        yield return charPedro.Say("Stop a la radio");

        //AudioController.Instance.StopSoundEffects("RadioStatic");

        charPedro.SetNameColor(Color.cyan);
        charPedro.SetDialogColor(Color.green);

        yield return charPedro.Say(lines);

        yield return alfonso.Say(lines);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Setting positions for Marcelo");
            charPedro.MoveToPosition(new Vector2(0.5f, 0.4f), 0.5f, true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Setting positions for Marcelo");
            charPatricia.MoveToPosition(new Vector2(0.5f, 0.4f), 0.5f, true);
        }


    }

    public void LoadFile(string filePath)
    {
        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);
        }
        catch
        {
            Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist!");
            return;
        }

        DialogController.Instance.Say(lines, filePath);
    }

}
