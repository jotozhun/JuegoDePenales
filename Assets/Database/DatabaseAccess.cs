using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseAccess : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://admin:admin2357@software2cluster.0u0gm.mongodb.net/<dbname>?retryWrites=true&w=majority");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;
    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("Juego");
        collection = database.GetCollection<BsonDocument>("users");

        var allUsers = collection.Find(new BsonDocument());
        var userList = allUsers.ToList();
        foreach(var user in userList)
        {
            Debug.Log(user.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
