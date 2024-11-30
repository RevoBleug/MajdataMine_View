using UnityEngine;

public class EachLineDrop : MonoBehaviour
{
    public float time;
    public int startPosition = 1;
    public int curvLength = 1;
    public float speed = 1;
    public bool canSVAffect;

    public Sprite[] curvSprites;
    private SpriteRenderer sr;

    private AudioTimeProvider timeProvider;

    // Start is called before the first frame update
    private void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();

        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = curvSprites[curvLength - 1];
        sr.forceRenderingOff = true;
    }

    // Update is called once per frame
    private void Update()
    {
        var timing = timeProvider.ScrollDist - timeProvider.GetPositionAtTime(time);
        var realtime = timeProvider.AudioTime - time;
        if(!canSVAffect)
        {
            timing = realtime;
        }
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        if (realtime > 0) Destroy(gameObject);
        if (distance < 1.225f)
        {
            distance = 1.225f;
            if (destScale > 0.3f) sr.forceRenderingOff = false;
            else sr.forceRenderingOff = true;
        }

        var lineScale = Mathf.Abs(distance / 4.8f);
        transform.localScale = new Vector3(lineScale, lineScale, 1f);
        transform.rotation = Quaternion.Euler(0, 0, -45f * (startPosition - 1));
    }
}