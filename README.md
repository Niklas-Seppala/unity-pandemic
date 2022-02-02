# Pandemic Game project using Unity game engine
This is a school project for a game course. Unitys Collaborate was used as VCS for the whole project, so this is just a dump.

Code is located in [Scripts folder](https://github.com/Niklas-Seppala/unity-pandemic/tree/readme/Assets/Scripts)
<br>

## Screenshot
![alt text](https://users.metropolia.fi/~niklasts/Unity/vids/pandemic_pic.PNG)


## Demo video
https://www.youtube.com/watch?v=bxoLbPEJe7I


## Some nice solutions

#### Generic DataAccess for SQLite
```csharp
public interface IDbModel
{
    int Id { get; }
    void CreateFromRow(IDataReader reader);
    void SetInsertParameters(DbCommand dbCommand);
    void SetUpdateParameters(DbCommand dbCommand);
}
```

All models implement the interface defined above, so they can provide means to
construct instances from IDataReader and also to serialize themselves to DbCommand.

#### Get player model from database
```csharp
public static T GetModel<T>(string where="") where T : class, IDbModel, new()
{
    using (var dbConnection = new SqliteConnection(connStr))
    using (var dbCommand = dbConnection.CreateCommand())
    {
        dbConnection.Open();
        if (SQL_STATEMENTS.SELECT.TryGetValue(typeof(T), out string query))
        {
            dbCommand.CommandText = query + where;            // Set SQL command
            using (var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())                          //While there are rows
                {
                    var model = new T();                      // Create new empty generic object
                    model.CreateFromRow(reader);              // Fill the model object with it's data
                    return model;
                }
            }
        }
    }
    return null;
}

public static IReadOnlyList<T> GetModels<T>(string where="") where T : class, IDbModel, new()
{                                                           // Constraints for type parameter T.
    var results = new List<T>();
    using (var dbConnection = new SqliteConnection(connStr))
    using (var dbCommand = dbConnection.CreateCommand())
    {
        dbConnection.Open();
        if (SQL_STATEMENTS.SELECT.TryGetValue(typeof(T), out string query)) // Get SQL command related to T.
        {
            dbCommand.CommandText = query + where;               // Set SQL command.
            using (var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())                             // While there are rows to read.
                {
                    var model = new T();                         // Create new empty generic object.
                    model.CreateFromRow(reader);                 // Fill the model object with it's data.
                    results.Add(model);                          // Add to results and continue.
                }
            }
        }
    }
    return results;
}
```

```csharp
player = DbAccess.GetModel<PlayerModel>(where: "Some additional where clause");
items = DbAccess.GetModels<ItemModel>(where: "Some additional where clause");
```

#### Generic Unit detection
```csharp
[Serializable]
public abstract class Unit : MonoBehaviour
{
    // ...
    public static bool Is<T>(GameObject obj, out T unit) where T : Unit
        => unit = obj.GetComponent<T>();
    // ...
}
```

Provides nice and efficient way for detecting the types of colliding units.

```csharp
// Enemy collisions
protected void OnCollisionEnter2D(Collision2D other)
{
    if (Unit.Is<Player>(other.gameObject, out var player))
    {
      // this unit has collided with player, and player GameObject has been cast to player
      // out variable and is ready to use.
    }
}
```

## WebGL build
Game is playable in browser.
<br>
https://users.metropolia.fi/~niklasts/Unity/WebBuild/

## Windows 64 build
https://users.metropolia.fi/~niklasts/Unity/WebBuild/Win_64/Pandemic_Win_64.zip
