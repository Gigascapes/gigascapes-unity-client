using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    public SpriteRenderer FillSprite;

    public float PaintSwapTime = 2f;
    float PaintSwapTimer = 0f;
    public float PaintSwapCooldownModifier = 0.5f;

    public Color PaintColor;

    public float PaintBlotInterval = 0.2f;
    float PaintBlotTimer = 0f;

    public GameObject PaintBlobPrefab;

    bool IsSwapping;
    Color ColorToSwapTo;
    int ZOrder = 0;

    void Update()
    {
        if (IsSwapping)
        {
            PaintSwapTimer += Time.deltaTime;
            if (PaintSwapTimer >= PaintSwapTime)
            {
                FillSprite.color = PaintColor = ColorToSwapTo;
                IsSwapping = false;
                PaintSwapTimer = 0f;
                Debug.Log("Swapped");
            }
        }
        else if (PaintSwapTimer > 0f)
        {
            PaintSwapTimer -= Time.deltaTime * PaintSwapCooldownModifier;
        }

        PaintBlotTimer += Time.deltaTime;
        if (PaintBlotTimer >= PaintBlotInterval)
        {
            ZOrder += 1;
            var newBlob = Instantiate(PaintBlobPrefab, transform.position, transform.rotation);
            newBlob.GetComponent<SpriteRenderer>().sortingOrder = ZOrder;
            newBlob.GetComponent<SpriteRenderer>().color = PaintColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var bucket = other.GetComponent<PaintBucket>();
        if (bucket != null)
        {
            IsSwapping = true;
            ColorToSwapTo = bucket.PaintColor;
            PaintSwapTimer = 0f;
            Debug.Log("Swapping");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var bucket = other.GetComponent<PaintBucket>();
        if (bucket != null)
        {
            IsSwapping = false;
            Debug.Log("Not Swapping");
        }
    }
}
