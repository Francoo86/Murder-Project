using System.Collections.Generic;

public class PlayerModel
{
    private static PlayerModel plyInstance = null;
    public static PlayerModel Instance { get; private set; }

    //Atributos del detective, es solo una instancia, y esto es especificamente lo que pide Inworld.
    //Si esto es JSON, y convertir esto a diccionario puede ser algo atadoso, lo ideal sería usar un diccionario.
    //FIXME: Realizar un casting de objeto.
    private Dictionary<string, string> DetectiveData = new Dictionary<string, string>();

    private PlayerModel() {
        DetectiveData["givenName"] = "Luis";
        DetectiveData["age"] = "30";
        DetectiveData["gender"] = "male";
        DetectiveData["role"] = "detective";
    }

    public Dictionary<string, string> GetData()
    {
        return DetectiveData;
    }
    public static PlayerModel GetInstance()
    {
        if(plyInstance == null)
        {
            plyInstance = new PlayerModel();
        }

        return plyInstance;
    }
}
