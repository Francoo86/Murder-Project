using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CandyCoded.env;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// API Wrapper para Inworld.
/// A pesar de que exista uno en Unity, solo se necesita algo que no sea tan complejo.
/// Sí, reinventamos la rueda...
/// </summary>
public class APIClient
{
    //Solo una instancia de este cliente es suficiente.
    //Solamente hacemos request de tipo POST, entonces un GET no será muy necesario a no ser
    //que la API RPC lo implemente.
    private static readonly HttpClient client = new HttpClient();
    private static readonly string API_URL = "https://studio.inworld.ai/v1/";

    //Variables de vital importancia.
    private string workspacePath;
    private string apiKey;

    public APIClient(string apiKey, string workspacePath) {
        SetAuthData(apiKey, workspacePath);
    }

    //Constructor vacio, carga las variables de entorno por defecto.
    public APIClient()
    {
        env.TryParseEnvironmentVariable("API_KEY", out apiKey);
        env.TryParseEnvironmentVariable("WORKSPACE_PATH", out workspacePath);
    }

    public void SetAuthData(string apiKey, string workspacePath)
    {
        this.apiKey = apiKey;
        this.workspacePath = workspacePath;
    }

    public override string ToString() => "Inworld API Client object.";

    private async Task<object> CallAPI(string endPoint, Dictionary<string, string> contentData, string sessionIdGrpc = null) {
        HttpRequestMessage message = new HttpRequestMessage();
        var serializedContent = JsonConvert.SerializeObject(contentData);

        message.RequestUri = new Uri($"{API_URL}{workspacePath}{endPoint}");
        message.Content = new StringContent(serializedContent, System.Text.Encoding.UTF8, "application/json");
        message.Method = HttpMethod.Post;

        message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        message.Headers.Add("authorization", apiKey);

        if (sessionIdGrpc != null)
        {
            message.Headers.Add("Grpc-Metadata-session-id", sessionIdGrpc);
        }

        var response = await client.SendAsync(message);
        string contentString = await response.Content.ReadAsStringAsync();

        if (contentString == null)
        {
            Debug.LogWarning("The response was null.");
            return null;
        }

        Debug.Log(contentString);
        return contentString;
    }

    private async Task<object> CallWithGRPC(string endPoint, string sessionId, Dictionary<string, string> contentData)
    {
        return await CallAPI(endPoint, contentData, sessionId);
    }

    public async Task<object> SendSimpleText(string text, string character, Dictionary<string, string> plyData) {
        string session_endpoint = $"/characters/{character}:simpleSendText";
        plyData.Add("text", text);

        //Debug.Log(text);
        return await CallAPI(session_endpoint, plyData);
    }

    //TODO: Refactor these methods (only parameters).
    public async Task<object> RequestCharacterSession(string character, Dictionary<string, string> plyData = null)
        => await CallAPI($"/characters/{character}:openSession", plyData);

    public async Task<object> SendPrompt(string sessId, string plyId, string text)
       => await CallWithGRPC($"/sessions/{sessId}/sessionCharacters/{plyId}:sendText", sessId, new Dictionary<string, string>() { { "text", text } });

    /*
    public async Task<string> SendTrigger(string sessId, string plyId, string triggerName, Dictionary<string, string> sceneParams = null) { 
        
    }*/
}
