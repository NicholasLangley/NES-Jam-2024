using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    int maxHealth;
    [SerializeField]
    HeartIcon heartPrefab;
    List<HeartIcon> hearts;

    [SerializeField]
    float offset;


    // Start is called before the first frame update
    void Start()
    {
        hearts = new List<HeartIcon>();
        maxHealth = 0;
    }

    public void addHeart()
    {
        hearts.Add(GameObject.Instantiate(heartPrefab));
        hearts[hearts.Count - 1].transform.SetParent(transform);

        Vector3 heartPos = transform.position;
        heartPos.x += (hearts.Count - 1) * offset;
        hearts[hearts.Count - 1].transform.position = heartPos;
        
        maxHealth += 2;
    }

    public void updateHealthBar(int currentHealth)
    {
        for(int i = 1; i <= hearts.Count; i++)
        {
            if(i * 2f <= currentHealth)
            {
                hearts[i - 1].updateHealth(HeartIcon.HEALTH_LEVEL.FULL);
            }
            else if (i * 2 - currentHealth == 1)
            {
                hearts[i - 1].updateHealth(HeartIcon.HEALTH_LEVEL.HALF);
            }
            else
            {
                hearts[i - 1].updateHealth(HeartIcon.HEALTH_LEVEL.EMPTY);
            }
        }
    }
}
