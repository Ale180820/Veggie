using MongoDB.Driver;
using System;


namespace VeggieAPI.Models {

    public class MongoHelper {

        public static IMongoClient client { get; set; }
        public static IMongoDatabase database { get; set; }
        public static string MongoConnection = "mongodb+srv://veggie_user:pass123@veggie.ioh5v.mongodb.net/<dbname>?retryWrites=true&w=majority";
        public static string MongoDatabase = "VeggieDB";
        public static IMongoCollection<VeggieBack.Models.User> users_collection { get; set; }

        internal static void ConnectToMongoService() {
            try { 
                client = new MongoClient(MongoConnection);
                database = client.GetDatabase(MongoDatabase);
            } catch (Exception) {
                Console.WriteLine("Error en conexión a base de datos");
            }
        }

    }
}
