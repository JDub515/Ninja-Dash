using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SavedData {

    public static int[] highScore;
    public static bool[] audioOptions;
    public static int[] challengeUnlocks;

    public static void SaveHighScore() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/highScores.gd");
        bf.Serialize(file, highScore);
        file.Close();
    }

    public static void LoadHighScore() {
        if (File.Exists(Application.persistentDataPath + "/highScores.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highScores.gd", FileMode.Open);
            highScore = (int[])bf.Deserialize(file);
            file.Close();
        } else {
            highScore = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            SaveHighScore();
        }
    }

    public static void SaveAudioOptions() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/audioOptions.gd");
        bf.Serialize(file, audioOptions);
        file.Close();
    }

    public static void LoadAudioOptions() {
        if (File.Exists(Application.persistentDataPath + "/audioOptions.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/audioOptions.gd", FileMode.Open);
            audioOptions = (bool[])bf.Deserialize(file);
            file.Close();
        } else {
            audioOptions = new bool[] { true, true };
            SaveAudioOptions();
        }
    }

    public static void SaveChallengeUnlocks() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/challengeUnlocks.gd");
        bf.Serialize(file, challengeUnlocks);
        file.Close();
    }

    public static void LoadChallengeUnlocks() {
        if (File.Exists(Application.persistentDataPath + "/challengeUnlocks.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/challengeUnlocks.gd", FileMode.Open);
            challengeUnlocks = (int[])bf.Deserialize(file);
            file.Close();
        } else {
            challengeUnlocks = new int[] { 0, 0, 0, 0, 0 };
            SaveChallengeUnlocks();
        }
    }
}
