using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 movement;

    public int score = 0;

    private int eyesDirection = 1;

    [SerializeField]
    private Transform eyes = null;

    [SerializeField]
    private Rigidbody2D rb;

    private Text scoreText;

    private WallState[,] maze;

    private bool immortal = false;
    private bool killed = false;

    public void init(WallState[,] m)
    {
        maze = m;
        setRandomPosition();
        StartCoroutine(Blink(3));

    }
    void Awake()
    {
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
    }
    // Update is called once per frame

    void setRandomPosition()
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        var pos = new Vector2(Mathf.Floor(Random.value * width), Mathf.Floor(Random.value * height));
        var position = new Vector3(-width / 2 + pos.x, -height / 2 + pos.y, 0);
        transform.position = position;
    }

    void Update()
    {
        var m = new Vector2();
        m.x = Input.GetAxisRaw("Horizontal");
        m.y = Input.GetAxisRaw("Vertical");
        if (m.x != movement.x)
        {
            movement.x = m.x;
            movement.y = 0;
            if (Mathf.Abs(transform.position.y - Mathf.Round(transform.position.y)) < 0.3f)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
            }
        }
        if (m.y != movement.y)
        {
            movement.y = m.y;
            movement.x = 0;
            if (Mathf.Abs(transform.position.x - Mathf.Round(transform.position.x)) < 0.3f)
            {
                transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
            }
        }
        if (movement.x > 0 && eyesDirection != 1)
        {
            eyesDirection = 1;
            eyes.localPosition = new Vector2(1.2f, 1f);

        }
        if (movement.x < 0 && eyesDirection != 0)
        {
            eyesDirection = 0;
            eyes.localPosition = new Vector2(0.8f, 1f);
        }
        if (movement.y > 0 && eyesDirection != 2)
        {
            eyesDirection = 2;
            eyes.localPosition = new Vector2(1f, 1.1f);
        }
        if (movement.y < 0 && eyesDirection != 3)
        {
            eyesDirection = 3;
            eyes.localPosition = new Vector2(1f, 0.8f);
        }
    }

    void FixedUpdate()
    {
        if (!killed)
        {
            rb.velocity = movement * 180f * Time.deltaTime;
        }
    }
    public void Kill()
    {
        if (!immortal)
        {
            killed = true;
            movement = new Vector2(0f, 0f);
            score = 0;
            scoreText.text = score.ToString();
            StartCoroutine(Blink(3));
            StartCoroutine(Teleport(0.3f));
        }
    }

    public void AddScore()
    {
        score += 1;
        scoreText.text = score.ToString();
    }
    IEnumerator Blink(float waitTime)
    {
        immortal = true;
        var endTime = Time.time + waitTime;
        var renderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        while (Time.time < endTime)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            renderer.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        immortal = false;
    }
    IEnumerator Teleport(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        setRandomPosition();
        killed = false;
    }

}
