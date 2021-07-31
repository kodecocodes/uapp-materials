using UnityEngine;
using System.Collections;
using System;

public class AmmunitionSpawner : MonoBehaviour
{
    public UIManager uiManager;

    public Transform spawnPoint;
    public Transform ammunitionParent;
    public float timeInSecondsBetweenCanSpawn = 2f;
    public GameObject[] ammunitionPrefabs;
    public AudioClip ammunitionCreationSound;
    public ParticleSystem smokeParticles;

    private float spawnTimer;
    private int amountOfAmmunitionSpawned;

    private void Start()
    {
        uiManager.InitialSetup(timeInSecondsBetweenCanSpawn);
    }

    public void AttemptAmmunitionSpawn()
    {
        if (spawnTimer == timeInSecondsBetweenCanSpawn)
        {
            SpawnRandomAmmunition();
        }
    }

    private void SpawnRandomAmmunition()
    {
        spawnTimer = 0f;

        int randomIndex = UnityEngine.Random.Range(0, ammunitionPrefabs.Length);
        Instantiate(ammunitionPrefabs[randomIndex], spawnPoint.position, ammunitionPrefabs[randomIndex].transform.localRotation, ammunitionParent);

        amountOfAmmunitionSpawned++;

        uiManager.UpdateAmmunitionText(amountOfAmmunitionSpawned);
        AudioSource.PlayClipAtPoint(ammunitionCreationSound, Camera.main.transform.position);
        smokeParticles.Play();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        spawnTimer = Mathf.Clamp(spawnTimer, 0, timeInSecondsBetweenCanSpawn);
        uiManager.UpdateAmmunitionTimerValue(spawnTimer);
    }
}