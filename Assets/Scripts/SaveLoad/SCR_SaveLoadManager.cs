using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class GameData
{
  public int money = 0;
  public int passenger = 0;
  public float[] playerPosition = new float[2];
}

public static class SaveManager
{
   public static GameData CurrentData = new GameData();

   private static string savePath = Application.persistentDataPath + "/save.json";

   private static byte[] GetHiddenKeyBytes()
   {
       return new byte[] { 69, 108, 32, 80, 115, 121, 32, 67, 111, 110, 103, 114, 111, 111, 33 };
   }

   [Serializable]
   private class SavePayload
   {
       public string gameDataJson;
       public string checksum;
   }

   public static void SaveGame()
   {
       string rawjson = JsonUtility.ToJson(CurrentData);
       string hash = GenerateHMAC(rawjson);

       SavePayload payload = new SavePayload()
       {
           gameDataJson = rawjson,
           checksum = hash
       };

       string finalSaveString = JsonUtility.ToJson(payload, true);
       File.WriteAllText(savePath, finalSaveString);
       Debug.Log("Secure game saved to: " + savePath);
   }

   public static void LoadGame()
   {
       if (File.Exists(savePath))
       {
           string finalSaveString = File.ReadAllText(savePath);
           SavePayload payload = JsonUtility.FromJson<SavePayload>(finalSaveString);

           string expectedHash = GenerateHMAC(payload.gameDataJson);
           if (expectedHash != payload.checksum)
           {
               Debug.LogError("Save file tampering detected! Loading fresh save.");
               CurrentData = new GameData();
               return;
           }

           CurrentData = JsonUtility.FromJson<GameData>(payload.gameDataJson);
           Debug.Log("Secure Game loaded!");
       }

       else
       {
           CurrentData = new GameData();
           Debug.LogWarning("No save file found, Make new Save file");   
       }
   }

   private static string GenerateHMAC(string text)
   {
       byte[] keyBytes = GetHiddenKeyBytes();
       byte[] textBytes = Encoding.UTF8.GetBytes(text);

       using (HMACSHA256 hmac = new HMACSHA256(textBytes))
       {
           byte[] hashByte = hmac.ComputeHash(keyBytes);
           return Convert.ToBase64String(hashByte);
       }
   }


}
