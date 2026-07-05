using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource musicExplore, musicBattle, musicVictory;
    public float volumeExplore, volumeBattle, volumeVictory;
    private void Awake()
    {
        if (MusicManager.instance == null)
            instance = this;
    }

    void Update()
    {
        musicExplore.volume = Mathf.MoveTowards(musicExplore.volume, volumeExplore, Time.deltaTime);
        musicBattle.volume = Mathf.MoveTowards(musicBattle.volume, volumeBattle, Time.deltaTime);
        musicVictory.volume = Mathf.MoveTowards(musicVictory.volume, volumeVictory, Time.deltaTime);
    }

    public void PlayExplore()
    {
        volumeVictory = 0f;
        volumeExplore = 0.5f;
        volumeBattle = 0f;
    }

    public void PlayBattle()
    {
        volumeVictory = 0f;
        volumeExplore = 0f;
        volumeBattle = 0.5f;
    }

    public void PlayVictory()
    {
        volumeVictory = 0.5f;
        musicVictory.volume = 0.5f;
        volumeExplore = 0f;
        volumeBattle = 0f;
        musicVictory.Play();
    }
}
