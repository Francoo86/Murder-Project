using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
//Should have an only instance of this thing.
public class HTTPConnection
{
    private readonly int[] SUCCESS_CODES = { 200, 201 };
    private static readonly HttpClient client = new HttpClient();
    private bool isValidRequest() {
        return true;
    }

    /*s
    public static async Task GetAsync(string url) { 
        using HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var JSONResponse = await response.Content.ReadAsStringAsync();

        return JSONResponse;
    }*/

}
