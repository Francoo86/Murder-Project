using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

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
        this.apiKey = apiKey;
        this.workspacePath = workspacePath;
    }

    public void SetAuthData(string apiKey, string workspacePath)
    {

    }

    public async Task CallAPI(string endPoint, Dictionary<string, string> contentData, Dictionary<string, string> customHeaders = null) {
        HttpRequestMessage message = new HttpRequestMessage();

        message.RequestUri = new Uri($"{API_URL}{workspacePath}{endPoint}");
        message.Content = new FormUrlEncodedContent(contentData);
        message.Method = HttpMethod.Post;

        Debug.Log(message.RequestUri);
        Debug.Log(message.Method);
        Debug.Log(message.Content);
        
        foreach(var item in contentData) {
            Debug.Log($"[{item.Key}: {item.Value}]");
        }

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

        message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //Preparar las cabeceras.
        //message.Headers.Add("Content-Type", "application/json");
        message.Headers.Add("authorization", apiKey);

        //Pythonic thing.
        if (customHeaders != null) {
            foreach(var item in customHeaders)
                message.Headers.Add(item.Key, item.Value);
        }

        var response = await client.SendAsync(message);
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public void CallWithGRPC(string endPoint, string sessionId, Dictionary<string, string> contentData)
    {

    }

    public async void SendSimpleText(string text, string character, Dictionary<string, string> plyData) {
        string session_endpoint = $"/characters/{character}:simpleSendText";
        plyData.Add("text", text);

        await CallAPI(session_endpoint, plyData);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
