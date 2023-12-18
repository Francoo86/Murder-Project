using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.env;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

//Creación de un cliente API pero con Corutinas (es mejor que usar HttpClient).
/// <summary>
/// Inworld API Wrapper to call its provided methods.
/// </summary>
public class APIClientV2 : MonoBehaviour
{
    private static readonly string API_URL = "https://studio.inworld.ai/v1/";
    private const string GRPC_ID = "Grpc-Metadata-session-id";
    private const string PREFERRED_METHOD = "POST";
    private static readonly HashSet<long> SUCCESFUL_CODES = new HashSet<long> { 200, 201 };
    public static APIClientV2 Instance {  get; private set; }

    //Variables de vital importancia.
    private string workspacePath;
    private string apiKey;

    /// <summary>
    /// Sets the authentication data, this might be assigned by default if a .env file is provided.
    /// </summary>
    /// <param name="apiKey">Inworld API Key.</param>
    /// <param name="workspacePath">The workspace provided by URL.</param>
    public void SetAuthData(string apiKey, string workspacePath)
    {
        this.apiKey = apiKey;
        this.workspacePath = workspacePath;
    }

    /// <summary>
    /// Calls the API by sending a request.
    /// </summary>
    /// <param name="endPoint">The endpoint path.</param>
    /// <param name="contentData">The content for doing POST request in dictionary format.</param>
    /// <param name="sessionIdGrpc">Session ID (if provided).</param>
    /// <param name="callback">Callback for doing any action with the requested data. The callback param is the data.</param>
    /// <returns>The IEnumerator related to the request.</returns>
    private IEnumerator CallAPI(string endPoint, Dictionary<string, string> contentData, string sessionIdGrpc = null, Action<string> callback = null)
    {
        //null thing makes unity crazy.
        string serializedContent = contentData != null ? JsonConvert.SerializeObject(contentData) : "";
        string fullUrl = $"{API_URL}{workspacePath}{endPoint}";

        Debug.LogWarning($"SERIALIZED: {serializedContent}");

        //HACK: Unity doesn't fix this since 2021.
        //Make the request.
        UnityWebRequest webRequest = UnityWebRequest.Put(fullUrl, serializedContent);
        webRequest.method = PREFERRED_METHOD;
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("authorization", apiKey);

        if (sessionIdGrpc != null)
        {
            webRequest.SetRequestHeader(GRPC_ID, sessionIdGrpc);
        }

        yield return webRequest.SendWebRequest();

        Debug.Log($"Last response: {webRequest.responseCode}");

        if (SUCCESFUL_CODES.Contains(webRequest.responseCode))
        {
            Debug.Log(webRequest.downloadHandler.text);
            callback?.Invoke(webRequest.downloadHandler.text);
        }
        else
            Debug.LogWarning("Can't connect to Inworld!");
    }

    /// <summary>
    /// Initializes the instance after the script is loaded.
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Initializes the Inworld API Wrapper object, also reads the key and workspace provided in the .env file.
    /// </summary>
    void Start()
    {
        env.TryParseEnvironmentVariable("API_KEY", out apiKey);
        env.TryParseEnvironmentVariable("WORKSPACE_PATH", out workspacePath);
    }

    /// <summary>
    /// Wrapped CallAPI method but with the support of the RPC session ID.
    /// </summary>
    /// <param name="endPoint">Endpoint path</param>
    /// <param name="sessionId">The current session.</param>
    /// <param name="contentData">The content provided to send in POST request.</param>
    /// <param name="callback">The callback that holds the data of the result.</param>
    /// <returns>The current IEnumerator requested.</returns>
    private IEnumerator CallWithGRPC(string endPoint, string sessionId, Dictionary<string, string> contentData, Action<string> callback)
    {
        if (sessionId != null)
        {
            Debug.LogWarning($"Session is not null {endPoint}, sessId: {sessionId}");
        }
        yield return CallAPI(endPoint, contentData, sessionId, callback);
    }

    /// <summary>
    /// Sends a single prompt without a session to a character.
    /// </summary>
    /// <param name="text">The text to send to the character.</param>
    /// <param name="character">The character who is receiving the message.</param>
    /// <param name="plyData">The player data for the character to be contextualized about player.</param>
    /// <returns>The current request.</returns>
    public IEnumerator SendSimpleText(string text, string character, Dictionary<string, string> plyData)
    {
        string session_endpoint = $"/characters/{character}:simpleSendText";
        plyData.Add("text", text);
        yield return CallAPI(session_endpoint, plyData, null, data => Debug.Log($"Current data: {data}"));
    }

    /// <summary>
    /// Requests a character session for the character based on the player's profile.
    /// </summary>
    /// <param name="character">Character name</param>
    /// <param name="plyData">The dictionary that holds information about player data.</param>
    /// <param name="callback">The callback that holds the response data.</param>
    /// <returns>The IEnumerator to yield with the request.</returns>
    public IEnumerator RequestCharacterSession(string character, Dictionary<string, string> plyData = null, Action<string> callback = null)
    {
        yield return StartCoroutine(CallAPI($"/characters/{character}:openSession", plyData, null, callback));
    }

    /// <summary>
    /// Sends a prompt to do conversations with a character.
    /// </summary>
    /// <param name="sessId">Session ID provided by Inworld.</param>
    /// <param name="plyId">Player ID provided by Inworld.</param>
    /// <param name="text">The text that will be sent to get a response from the character.</param>
    /// <param name="callback">The callback that holds the response data.</param>
    /// <returns>The IEnumerator to yield with the request.</returns>
    public IEnumerator SendPrompt(string sessId, string plyId, string text, Action<string> callback)
    {
        string promptEndpoint = $"/sessions/{sessId}/sessionCharacters/{plyId}:sendText";
        yield return StartCoroutine(CallWithGRPC(promptEndpoint, sessId, new Dictionary<string, string>() { { "text", text } }, callback));
    }
}