using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CHARACTERS;

public class CommandCharacterExtension : CommandDBExtension
{
	//Activar personajes y eso.
	private static string[] ENABLED_CHARACTER = new string[] { "-e", "-enabled" };
	private static string[] INMEDIATE_APPEARING = new string[] { "-i", "-inmediate" };
	//Posiciones.
	private static string XPOS = "-x";
	private static string YPOS = "-y";
	private static string[] SPEED_PARAM = new string[] { "-s", "-speed" };
	private static string SMOOTH_PARAM = "-smoothness";

	new public static void Extend(CommandDB commandDB)
	{
		commandDB.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
		commandDB.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
		commandDB.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));

		//The technological plus.
		commandDB.AddCommand("talkcharacter", new Func<string, IEnumerator>(TalkWithCharacter));
	}

	private static void CreateCharacter(string[] data)
	{
		string charName = data[0];
		bool enabled = false;
		bool inmediate = false;
		Character character = CharacterController.Instance.CreateCharacter(charName, enabled);

		var parameters = ConvertToParams(data);
		parameters.TryGetValue(ENABLED_CHARACTER, out enabled, false);
		parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

		Debug.Log($"Creating from command {charName}");

		if (!enabled) return;

		if (inmediate)
			character.IsVisible = true;
		else
			character.Show();
	}

	//Trying.
	private static IEnumerator MoveCharacter(string[] data)
	{
		string charName = data[0];

		Character character = CharacterController.Instance.GetCharacter(charName);

		if (character == null) yield break;

		float x = 0, y = 0;
		float speed = 0;
		bool smooth = false;
		bool inmediate = false;

		var parameters = ConvertToParams(data);
		//Obtener posicion en X.
		parameters.TryGetValue(XPOS, out x);
		//En Y.
		parameters.TryGetValue(YPOS, out y);
		//Ver si se mueve de inmediato.
		parameters.TryGetValue(SPEED_PARAM, out speed, defaultVal: 1);
		//Queremos suavidad?
		parameters.TryGetValue(SMOOTH_PARAM, out smooth, false);
		//Altoque?
		parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

		Vector2 pos = new Vector2(x, y);
		if (inmediate)
			character.SetPos(pos);
		else
			CommandController.Instance.AddTerminationActionToActualProcess(() => { character.SetPos(pos); });
		yield return character.MoveToPosition(pos, speed, smooth);

		//yield return null;
	}

	private static IEnumerator ShowAll(string[] data)
	{
		List<Character> allCharacters = new List<Character>();
		bool inmediate = false;

		foreach (string character in data)
		{
			Character currentCharacter = CharacterController.Instance.GetCharacter(character, create: false);

			if (currentCharacter != null)
				allCharacters.Add(currentCharacter);
		}

		if (allCharacters.Count == 0)
		{
			yield break;
		}

		var parameters = ConvertToParams(data);
		parameters.TryGetValue(INMEDIATE_APPEARING, out inmediate, false);

		foreach (Character character in allCharacters)
		{
			if (!inmediate)
				character.Hide();
			else
				character.IsVisible = true;
		}

		if (!inmediate)
		{
			while (allCharacters.Any(c => c.IsHiding))
			{
				yield return null;
			}
		}
	}

	private const string STOP_ID = "stop";
	private static IEnumerator TalkWithCharacter(string characterName)
	{
		characterName = characterName.ToLower();
		PromptPanel panel = PromptPanel.Instance;

		//Loads a new session.
		AISessionManager sess = new AISessionManager(characterName);
		CoroutinePrompt prompt = CoroutinePrompt.GetInstance();
		prompt.InjectSession(sess);

		Character character = CharacterController.Instance.GetCharacter(characterName, true);

		yield return character.Show();
		
		//FIXME: Save those lines in the conversation.
		while (true)
		{
			panel.Show("What do you want to ask me?");

			while (panel.IsWaitingOnUserInput)
				yield return null;

			if (panel.LastInput == STOP_ID)
			{
				yield break;
			}
		   
			yield return prompt.Talk(panel.LastInput);

			while (prompt.IsStillFetching)
				yield return null;

			prompt.Interact(characterName);
			character.OnExpressionReceive(0, prompt.GetResponseExpression());

			//HACK: STOPS THE OTHER COROUTINES FOR THIS ONE.
			yield break;
		}

		Debug.Log("<color=#008000>Spitting fax!!!!</color>");
		Debug.Log($"STILL FETCHING??? {prompt.IsStillFetching}");
		//yield return null;
	}

	public static void HideAll(string[] data)
	{

	}
}
