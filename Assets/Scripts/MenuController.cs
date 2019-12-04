using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour {

    public bool mainMenu;

    public TextMeshProUGUI highScoreText;
    public GameObject player;
    public GameObject pauseButton;
    public GameObject pauseMenu;

    private GameObject currentBackButton;

    public TextMeshProUGUI musicText;
    public TextMeshProUGUI soundText;
    public AudioMixer mixer;

    public GameObject bossRushLock;
    public GameObject swordOnlyLock;
    public GameObject speedModeLock;
    public GameObject hardModeLock;

    // Use this for initialization
    void Start () {
        SavedData.LoadHighScore();
        SavedData.LoadChallengeUnlocks();
        highScoreText.text = "High Score: " + SavedData.highScore[0];
        if (mainMenu) {
            SavedData.LoadAudioOptions();
            musicText.text = SavedData.audioOptions[0] ? "Music: On" : "Music: Off";
            soundText.text = SavedData.audioOptions[1] ? "Sound: On" : "Sound: Off";
            if (SavedData.audioOptions[0]) {
                mixer.SetFloat("MusicVolume", 0);
            } else {
                mixer.SetFloat("MusicVolume", -80);
            }
            if (SavedData.audioOptions[1]) {
                mixer.SetFloat("SoundVolume", 0);
            } else {
                mixer.SetFloat("SoundVolume", -80);
            }
            if (SavedData.challengeUnlocks[0] >= 10) {
                bossRushLock.SetActive(false);
            }
            if (SavedData.challengeUnlocks[1] >= 3) {
                swordOnlyLock.SetActive(false);
            }
            if (SavedData.challengeUnlocks[2] >= 3) {
                speedModeLock.SetActive(false);
            }
            if (SavedData.challengeUnlocks[3] >= 1) {
                hardModeLock.SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().name == "Game") {
                if (Time.timeScale == 1) {
                    Time.timeScale = 0;
                    player.GetComponent<PlayerController>().disabled = true;
                    pauseButton.SetActive(false);
                    pauseMenu.SetActive(true);
                } else {
                    Time.timeScale = 1;
                    player.GetComponent<PlayerController>().disabled = false;
                    pauseButton.SetActive(true);
                    pauseMenu.SetActive(false);
                }
            } else {
                if (currentBackButton == null) {
                    Application.Quit();
                } else {
                    ExecuteEvents.Execute(currentBackButton, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }

    public void StartPressed() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void ExitPressed() {
        Application.Quit();
    }

    public void PausePressed() {
        Time.timeScale = 0;
        player.GetComponent<PlayerController>().disabled = true;
    }

    public void ResumePressed() {
        StartCoroutine("Resume");
    }

    private IEnumerator Resume() {
        yield return null;
        Time.timeScale = 1;
        player.GetComponent<PlayerController>().disabled = false;
    }

    public void MenuPressed() {
        SavedData.SaveChallengeUnlocks();
        SceneManager.LoadScene("Menu");
    }

    public void SetCurrentBackButton(GameObject backButton) {
        currentBackButton = backButton;
    }

    public void ResetCurrentBackButton() {
        currentBackButton = null;
    }

    public void SetGameMode(int g) {
        LevelCreation.gameMode = g;
    }

    public void MusicToggle() {
        SavedData.audioOptions[0] = !SavedData.audioOptions[0];
        if (SavedData.audioOptions[0]) {
            mixer.SetFloat("MusicVolume", 0);
        } else {
            mixer.SetFloat("MusicVolume", -80);
        }
        musicText.text = SavedData.audioOptions[0] ? "Music: On" : "Music: Off";
        SavedData.SaveAudioOptions();
    }

    public void SoundToggle() {
        SavedData.audioOptions[1] = !SavedData.audioOptions[1];
        if (SavedData.audioOptions[1]) {
            mixer.SetFloat("SoundVolume", 0);
        } else {
            mixer.SetFloat("SoundVolume", -80);
        }
        soundText.text = SavedData.audioOptions[1] ? "Sound: On" : "Sound: Off";
        SavedData.SaveAudioOptions();
    }
}
