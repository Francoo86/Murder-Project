using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Le da acceso centralizado a las variables en los archivos de dialogo.
/// TODO: Ver si se puede realizar algo parecido pero con un interprete de Lua.
/// </summary>
public class VariableStore
{
    //Clase acoplada moment.
    public class Database
    {
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

    public abstract class Variable {
        public abstract object Get();
        public abstract void Set(object value);
    }

    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> getter;
        private Action<T> setter;

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

    public static bool CreateDatabase(string databaseName)
    {
        if (!databases.ContainsKey(databaseName))
        {
            databases[databaseName] = new Database(databaseName);
            return true;
        }

        return false;
    }

    public static Database GetDatabase(string databaseName)
    {
        if (databaseName == string.Empty)
            return defaultDatabase;

        if (!databases.ContainsKey(databaseName))
            CreateDatabase(databaseName);

        return databases[databaseName];
    }

    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[DEFAULT_DATABASE] = new Database(DEFAULT_DATABASE);
    }

    public static bool HasVariable(string name)
    {
        (string[] _, Database db, string varName) = ExtractInfo(name);
        return db.variables.ContainsKey(varName);
    }

    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string varName) = ExtractInfo(name);

        if(db.variables.ContainsKey(varName))
            db.variables.Remove(varName);
    }

    public static void PrintAllDatabases()
    {
        foreach (KeyValuePair<string, Database> dbEntry in databases)
        {
            Debug.Log($"Database: '<color=#FFB145>{dbEntry.Key}</color>");
        }
    }

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
    public static bool CreateVariable<T>(string name, T defaulValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string varName) = ExtractInfo(name);

        if (db.variables.ContainsKey(varName))
            return false;

        db.variables[varName] = new Variable<T>(defaulValue, getter, setter);
        return true;
    }

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

    private static (string[], Database, string) ExtractInfo(string name)
    {
        //Para cada BD agregar un elemento. Onda, DB1.VarName = 1.
        string[] parts = name.Split(DATABASE_RELATIONAL_ID);
        Database db = GetDatabase(parts[0]);
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }
}
