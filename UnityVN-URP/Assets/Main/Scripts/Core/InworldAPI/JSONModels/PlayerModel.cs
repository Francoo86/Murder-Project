public class PlayerModel
{
    public static PlayerModel plyInstance = null;

    //Atributos del detective, es solo una instancia, y esto es especificamente lo que pide Inworld.
    private string givenName;
    private float age;
    private string gender;
    private string role;

    public string Name { get { return givenName; } set { givenName = value; } }
    public float Age { get { return age; } set { age = value; } }
    public string Gender { get { return gender; } set { gender = value; } }
    public string Role {get { return role; } set { role = value; } }

    private PlayerModel() {
        givenName = "Luis";
        age = 30;
        gender = "male";
        role = "detective";
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
