using System.Collections;
using UnityEngine;

public class CardFlipper : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer { get { return GetComponent<SpriteRenderer>(); } }
    private float originalXScale;
    //private float originalZPosition = 0.01f;

    public AnimationCurve scaleCurve;
    public float duration = 1f;

    void Awake()
    {
        originalXScale = transform.localScale.x;
        //originalZPosition = transform.localPosition.z;
    }

    //public void FlipCard(Sprite startImage, Sprite endImage, int cardIndex)
    //{
    //    StopCoroutine(Flip(startImage, endImage));
    //    StartCoroutine(Flip(startImage, startImage));
    //}

    public IEnumerator Flip(Sprite startImage, Sprite endImage, float duration)
    {
        SpriteRenderer.sprite = startImage;

        float time = 0f;
        while (time <= duration)
        {
            time += Time.deltaTime / duration;
            float scale = scaleCurve.Evaluate(time);

            // shrinking/expand the card horizontally (i.e. only rescaling the X)
            Vector3 localScale = transform.localScale;
            localScale.x = scale * originalXScale;
            transform.localScale = localScale;

            // moving the card towards/away from the camera (i.e. only moving on the Z axis)        
            Vector3 localPosition = transform.localPosition;
            localPosition.z = (-1 + scale);  //* originalZPosition;  // because originalZPosition is most likely 0
            transform.localPosition = localPosition;

            // if card is halfway through transition, switch card face to card back
            if (time >= (duration / 2))
            {
                SpriteRenderer.sprite = endImage;
            }

            yield return new WaitForFixedUpdate();  // yield pauses execution of the function at this point until the next frame
        }
    }
}