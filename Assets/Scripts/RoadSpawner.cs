using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject[] section;
    public GameObject coin;
    public Vector2 numberofCoins;

    public List<GameObject> newCoins;

    public int zPos = 113;
    public bool creatingSection = false;
    public int secNum;

    private void Start()
    {
        int NewNumberOfCoins = (int)UnityEngine.Random.Range(numberofCoins.x, numberofCoins.y);
        for (int i = 0; i < NewNumberOfCoins; i++)
        {
            newCoins.Add(Instantiate(coin,transform));
            newCoins[i].SetActive(false);
        }
        PositionCoins(); 
    }
    private void Update()
    {
        if (!creatingSection)
        {
            StartCoroutine(GenerateSection());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PositionCoins();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            RoadSpawner roadSpawner = FindObjectOfType<RoadSpawner>();
            if (roadSpawner != null)
            {
                roadSpawner.DestroySections();
            }
        }
    }
    IEnumerator GenerateSection()
    {
        creatingSection = true;
        secNum = UnityEngine.Random.Range(0, section.Length);
        GameObject newSection = Instantiate(section[secNum], new Vector3(-9.4f, 0, zPos), Quaternion.identity);
        newSection.transform.Rotate(Vector3.up, 90f);
        zPos += 100;
        yield return new WaitForSeconds(10); //bekleme s�resi
        creatingSection = false;
    }
    bool IsPositionSafe(Vector3 position, float safeDistance) //coinlerin engellerin i�erisinde olu�mamas� i�in yazm�� oldu�um kod 
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Engel");
        foreach (var obstacle in obstacles)
        {
            if (Vector3.Distance(position, obstacle.transform.position) < safeDistance)
            {
                return false; // Bir engelin �ok yak�nsa coini olu�turma
            }
        }
        return true; // E�er hi�bir engelle belirli bir mesafe i�inde de�ilse coini olu�tur
    }
    void PositionCoins()
    {
        float minZPos = 10f;
        float[] possibleXPositions = new float[] { -3f, 0f, 3f };

        for (int i = 0; i < newCoins.Count; i++)
        {
            Vector3 newPosition;
            do
            {
                float maxZPos = minZPos + 10f;
                float randomZPos = UnityEngine.Random.Range(minZPos, maxZPos);
                float randomXPos = possibleXPositions[UnityEngine.Random.Range(0, possibleXPositions.Length)];
                newPosition = new Vector3(randomXPos, transform.position.y, randomZPos);
            }
            
            while (!IsPositionSafe(newPosition, 2f)); 

            newCoins[i].transform.position = newPosition;
            newCoins[i].SetActive(true);
            minZPos = newPosition.z + 1; // Bir sonraki coin i�in Z pozisyonunu g�nceller
        }
    }
    void DestroySections()
    {
        GameObject[] sections = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject section in sections)
        {
            if (section.transform.position.z < (zPos - 200)) // E�er karakter zPos - 200'den �nceki bir yolu ge�tiyse, o yolu yok etmek 
            {
                Destroy(section);
            }
        }
    }
   
}



