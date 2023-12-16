using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.env;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

//Creación de un cliente API pero con Corutinas (es mejor que usar HttpClient).
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

    public void SetAuthData(string apiKey, string workspacePath)
    {
        this.apiKey = apiKey;
        this.workspacePath = workspacePath;
    }

    IEnumerator CallAPI(string endPoint, Dictionary<string, string> contentData, string sessionIdGrpc = null, Action<string> callback = null)
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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        env.TryParseEnvironmentVariable("API_KEY", out apiKey);
        env.TryParseEnvironmentVariable("WORKSPACE_PATH", out workspacePath);
    }

    private IEnumerator CallWithGRPC(string endPoint, string sessionId, Dictionary<string, string> contentData, Action<string> callback)
    {
        if (sessionId != null)
        {
            Debug.LogWarning($"Session is not null {endPoint}, sessId: {sessionId}");
        }
        yield return CallAPI(endPoint, contentData, sessionId, callback);
    }

    public IEnumerator SendSimpleText(string text, string character, Dictionary<string, string> plyData)
    {
        string session_endpoint = $"/characters/{character}:simpleSendText";
        plyData.Add("text", text);
        yield return CallAPI(session_endpoint, plyData, null, data => Debug.Log($"Current data: {data}"));
    }

    //TODO: Refactor these methods (only parameters).
    public IEnumerator RequestCharacterSession(string character, Dictionary<string, string> plyData = null, Action<string> callback = null)
    {
        yield return StartCoroutine(CallAPI($"/characters/{character}:openSession", plyData, null, callback));
    }

    public IEnumerator SendPrompt(string sessId, string plyId, string text, Action<string> callback)
    {
        string promptEndpoint = $"/sessions/{sessId}/sessionCharacters/{plyId}:sendText";
        yield return StartCoroutine(CallWithGRPC(promptEndpoint, sessId, new Dictionary<string, string>() { { "text", text } }, callback));
    }
}
