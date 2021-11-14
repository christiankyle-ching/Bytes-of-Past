using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class CustomSerializer
{
    // Players
    public static byte[] SerializePlayers(Dictionary<uint, string> _object)
    {
        if (_object == null) return null;

        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, _object);

            return memoryStream.ToArray();
        }
    }

    public static Dictionary<uint, string> DeserializeDict_uint_string(byte[] dataStream)
    {
        if (dataStream == null) return null;

        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Dictionary<uint, string>)binaryF.Deserialize(memoryStream);
        }
    }

    // PlayerHands
    public static byte[] SerializePlayerHands(Dictionary<uint, List<int>> _object)
    {
        if (_object == null) return null;

        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, _object);

            return memoryStream.ToArray();
        }
    }

    public static Dictionary<uint, List<int>> DeserializeDict_uint_listInt(byte[] dataStream)
    {
        if (dataStream == null) return null;

        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Dictionary<uint, List<int>>)binaryF.Deserialize(memoryStream);
        }
    }

    // PlayerTrades
    public static byte[] SerializePlayerTrades(Dictionary<uint, int> _object)
    {
        if (_object == null) return null;

        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, _object);

            return memoryStream.ToArray();
        }
    }

    public static Dictionary<uint, int> DeserializeDict_uint_int(byte[] dataStream)
    {
        if (dataStream == null) return null;

        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Dictionary<uint, int>)binaryF.Deserialize(memoryStream);
        }
    }

}
