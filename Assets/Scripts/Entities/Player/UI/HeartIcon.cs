using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartIcon : MonoBehaviour
{
    Image image;
    [SerializeField]
    Sprite emptyHeartSprite, halfHeartSprite, fullHeartSprite;

    public enum HEALTH_LEVEL { EMPTY, HALF, FULL}

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.AddComponent<Image>();
        image.sprite = fullHeartSprite;
    }

    public void updateHealth(HEALTH_LEVEL level)
    {
        switch (level)
        {
            case HEALTH_LEVEL.EMPTY:
                image.sprite = emptyHeartSprite;
                break;
            case HEALTH_LEVEL.HALF:
                image.sprite = halfHeartSprite;
                break;
            case HEALTH_LEVEL.FULL:
                image.sprite = fullHeartSprite;
                break;
            default:
                break;
        }

    }
}
