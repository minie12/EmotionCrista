using UnityEngine;

public class PatternBubble : MonoBehaviour
{
    private RectTransform r_transform;
    private float speed = 0.06f;
    public Sprite[] bubbles;
    private int bubbleId;
    private bool clicked;

    void Awake()
    {
        r_transform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        clicked = false;
        bubbleId = Random.Range(0, bubbles.Length);
        speed = Random.Range(0.06f, 0.08f);
        this.GetComponent<SpriteRenderer>().sprite = bubbles[bubbleId];
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, speed, 0));

        if (r_transform.anchoredPosition.y > 670.0f && !clicked)
        {
            GameObject.Find("MiniManager").GetComponent<PatternBlue>().SetFailGaugeMount(1);
            Destroy(this.gameObject);
        }

        Check2DObjectClicked();
    }

    void Check2DObjectClicked()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Mouse is pressed down");
            Camera cam = Camera.main;

            //Raycast depends on camera projection mode
            Vector2 origin;
            Vector2 dir = Vector2.zero;

            if (cam.orthographic)
            {
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                origin = ray.origin;
                dir = ray.direction;
            }

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, Mathf.Infinity, LayerMask.GetMask("Bubble"));

            //Check if we hit anything
            if (hit && hit.collider.gameObject == this.gameObject && !clicked)
            {
                clicked = true;
                this.GetComponent<Animator>().enabled = true;
                this.GetComponent<Animator>().Play($"blue_bubble_{bubbleId + 1}", 0, 0.0f);
                Invoke(nameof(DestroyObject), 0.5f);
            }
        }
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}

