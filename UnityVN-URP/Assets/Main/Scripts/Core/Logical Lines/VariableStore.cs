using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Le da acceso centralizado a las variables en los archivos de dialogo.
// TODO: Ver si se puede realizar algo parecido pero con un interprete de Lua.
/// <summary>
/// Class that acts like a controller of variables.
/// </summary>
public class VariableStore
{
    //Clase acoplada moment.
    /// <summary>
    /// Database that stores variables.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Class that stores its correspondent variables.
        /// </summary>
        /// <param name="name">Database name</param>
        public Database(string name)
        {
            this.name = name;
            //Siento que esto no tiene mucho sentido, pero al finalizar estos videos lo corregire.
            variables = new Dictionary<string, Variable> ();

        }
        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
    }

    private const string DEFAULT_DATABASE = "Default";
    public static readonly char DATABASE_RELATIONAL_ID = '.';

    private static Dictionary<string, Database> databases = new Dictionary<string, Database>() { { DEFAULT_DATABASE, new Database(DEFAULT_DATABASE) } };
    private static Database defaultDatabase => databases[DEFAULT_DATABASE];

    /// <summary>
    /// Helper class to define Variable types.
    /// </summary>
    public abstract class Variable {
        /// <summary>
        /// The variable getter function.
        /// </summary>
        /// <returns>The variable value based on its type.</returns>
        public abstract object Get();
        /// <summary>
        /// The variable setter function, changes the value.
        /// </summary>
        /// <param name="value">The new value based on the variable type.</param>
        public abstract void Set(object value);
    }

    /// <summary>
    /// Class that defines variables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> getter;
        private Action<T> setter;

        /// <summary>
        /// The variable constructor.
        /// </summary>
        /// <param name="defaultVal">Default value of the variable.</param>
        /// <param name="getter">The getter function.</param>
        /// <param name="setter">The setter function.</param>
        public Variable(T defaultVal = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultVal;

            if (getter != null)
                this.getter = getter;
            else
                this.getter = () => value;

            if (setter != null)
                this.setter = setter;
            else
                this.setter = newVal => value = newVal;
        }
        public override object Get()
        {
            return getter();
        }

        public override void Set(object value)
        {
            setter((T)value);
        }
    }

    /// <summary>
    /// Creates a database object with the provided name and stores it on the databases list. It will return false if a database has an existant name.
    /// </summary>
    /// <param name="databaseName">The database name.</param>
    /// <returns>Wether the database was sucessfully created.</returns>
    public static bool CreateDatabase(string databaseName)
    {
        if (!databases.ContainsKey(databaseName))
        {
            databases[databaseName] = new Database(databaseName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves a database from the list. If the database with the provided name doesn't exists, it will create it automatically.
    /// </summary>
    /// <param name="databaseName">The database name.</param>
    /// <returns>The database object.</returns>

    public static Database GetDatabase(string databaseName)
    {
        if (databaseName == string.Empty)
            return defaultDatabase;

        if (!databases.ContainsKey(databaseName))
            CreateDatabase(databaseName);

        return databases[databaseName];
    }
    /// <summary>
    /// Clears all databases and removes every variable from them.
    /// </summary>
    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[DEFAULT_DATABASE] = new Database(DEFAULT_DATABASE);
    }

    /// <summary>
    /// Checks if some variable exists (it may exists on a database or in the default one).
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <returns>Wether the variable exists or not.</returns>
    public static bool HasVariable(string name)
    {
        (string[] _, Database db, string varName) = ExtractInfo(name);
        return db.variables.ContainsKey(varName);
    }

    /// <summary>
    /// Removes a variable from a database (or the default one).
    /// </summary>
    /// <param name="name">Variable name.</param>
    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string varName) = ExtractInfo(name);

        if(db.variables.ContainsKey(varName))
            db.variables.Remove(varName);
    }

    /// <summary>
    /// Helper function that prints all databases on the list.
    /// </summary>
    public static void PrintAllDatabases()
    {
        foreach (KeyValuePair<string, Database> dbEntry in databases)
        {
            Debug.Log($"Database: '<color=#FFB145>{dbEntry.Key}</color>");
        }
    }

    /// <summary>
    /// Prints all variables of an specific database, if not specified, then prints every variable of every database.
    /// </summary>
    /// <param name="database">Database object if provided.</param>
    public static void PrintAllVariables(Database database = null)
    {
        if(database != null)
        {
            PrintAllDatabaseVariables(database);
            return;
        }

        foreach (var dbEntry in databases)
        {
            PrintAllDatabaseVariables(dbEntry.Value);
        }
    }

    /// <summary>
    /// Helper function that prints all variable of a specified database.
    /// </summary>
    /// <param name="database">Database where we are fetching from.</param>
    public static void PrintAllDatabaseVariables(Database database)
    {
        foreach (KeyValuePair<string, Variable> variableEntry in database.variables)
        {
            string varName = variableEntry.Key;
            object varValue = variableEntry.Value.Get();

            Debug.Log($"Database name: {database.name} Variable name: {varName} Variable Value: {varValue}");
            //Debug.Log($"Database: {dbEntry.Key}");
        }
    }

    //MANEJO DE VARIABLES.
    /// <summary>
    /// Creates a variable.
    /// By default if a database is not provided in this format DBName.VarName then it will be assigned to the default DB.
    /// </summary>
    /// <typeparam name="T">Type of the value (int, float, bool, string).</typeparam>
    /// <param name="name">Variable name.</param>
    /// <param name="defaulValue">Default value of the variable (based on its type).</param>
    /// <param name="getter">The Getter function of the variable, will be assigned by default if not provided.</param>
    /// <param name="setter">The Setter function of the variable, will be assigned by default if not provided</param>
    /// <returns>Wether the variable was created succesfully or not.</returns>
    public static bool CreateVariable<T>(string name, T defaulValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string varName) = ExtractInfo(name);

        if (db.variables.ContainsKey(varName))
            return false;

        db.variables[varName] = new Variable<T>(defaulValue, getter, setter);
        return true;
    }

    /// <summary>
    /// Tries to retrieve a value by the variable getter.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="variable">Where the variable should be stored (the type should be specified).</param>
    /// <returns>Success.</returns>
    public static bool TryGetValue(string name, out object variable)
    {
        (string[] _, Database db, string varName) = ExtractInfo(name);
        if(!db.variables.ContainsKey(varName))
        {
            variable = null;
            return false;
        }

        variable = db.variables[varName].Get();
        return true;
    }

    /// <summary>
    /// Tries to set the value of a variable according to its type.
    /// </summary>
    /// <typeparam name="T">Any type of variable (float, int, string, bool).</typeparam>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The value to set to that variable by the supported types.</param>
    /// <returns></returns>
    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] _, Database db, string varName) = ExtractInfo(name);
        if (!db.variables.ContainsKey(varName))
        {
            return false;
        }

        db.variables[varName].Set(value);
        return true;
    }

    /// <summary>
    /// Extracts data about a variable by passing the name.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <returns>Parts of the variable (The DB if has any, the variable), the database used by the variable, the variable name.</returns>
    private static (string[], Database, string) ExtractInfo(string name)
    {
        //Para cada BD agregar un elemento. Onda, DB1.VarName = 1.
        string[] parts = name.Split(DATABASE_RELATIONAL_ID);
        Database db = GetDatabase(parts[0]);
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }
}
