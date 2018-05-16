using System.Collections;
using UnityEngine;

public class CardFlipper : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CardModel model;
    private float originalXScale;
    private float originalZPosition;

    public AnimationCurve scaleCurve;
    public float duration = 0.5f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        model = GetComponent<CardModel>();
        originalXScale = transform.localScale.x;
        originalZPosition = transform.localPosition.z;
    }

    public void FlipCard(Sprite startImage, Sprite endImage, int cardIndex)
    {
        // Co-Routines run over multiple frames
        StopCoroutine(Flip(startImage, endImage, cardIndex));
        StartCoroutine(Flip(startImage, endImage, cardIndex));
    }

    IEnumerator Flip(Sprite startImage, Sprite endImage, int cardIndex)
    {
        spriteRenderer.sprite = startImage;

        float time = 0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / duration;
            float scale = scaleCurve.Evaluate(time);

            // shrinking/expand the card horizontally (i.e. only rescaling the X)
            Vector3 localScale = transform.localScale;
            localScale.x = scale * originalXScale;
            transform.localScale = localScale;

            // moving the card towards/away from the camera (i.e. only moving on the Z axis)
            Vector3 localPosition = transform.localPosition;
            localPosition.z = scale * originalZPosition;
            transform.localPosition = localPosition;

            // if card is halfway through transition, switch card face to card back
            if (time >= 0.5f)
            {
                spriteRenderer.sprite = endImage;
            }

            yield return new WaitForFixedUpdate(); // yield pauses execution of the function at this point until the next frame
        }

        if (cardIndex == -1)
            model.ToggleFace(false);
        else
        {
            model.cardIndex = cardIndex;
            model.ToggleFace(true);
        }
    }
}
