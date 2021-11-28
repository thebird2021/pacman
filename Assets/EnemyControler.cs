using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControler : MonoBehaviour
{
    private Vector2 movement = new Vector2(0, 0);
    [SerializeField]
    [Range(0, 500)]
    private float speed = 100f;

    public int score = 0;

    [SerializeField]
    private Transform directionPrefab;

    private Transform dir_marker;

    private int eyesDirection = -1;

    [SerializeField]
    private Transform eyes = null;

    [SerializeField]
    private Rigidbody2D rb;
    private Vector3 mazePosition;
    private WallState[,] maze;
    public float[,] mazeHeat;

    public void init(WallState[,] m)
    {
        maze = m;
        mazeHeat = new float[maze.GetLength(0), maze.GetLength(1)];
        //dir_marker = Instantiate(directionPrefab, transform.parent);
        getRoute();
    }
    private Vector2 choiseDirection(List<Vector2> posibleDirections)
    {
        var choise = (int)Mathf.Floor(Random.value * posibleDirections.Count);
        if (posibleDirections.Count > 1)
        {
            var inverted = new Vector2(0 - posibleDirections[choise].x, 0 - posibleDirections[choise].y);
            if (movement != inverted)
            {
                return posibleDirections[choise];
            }
            else
            {
                return choiseDirection(posibleDirections);
            }
        }
        else
        {
            return posibleDirections[choise];
        }


    }

    public void getRoute()
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        Vector3 position = getMazePosition(transform.position);
        mazePosition = position;
        WallState state = maze[(int)position.x, (int)position.y];
        var posibleDirections = new List<Vector2>();
        var old_movement = movement;

        if (!state.HasFlag(WallState.LEFT))
        {
            posibleDirections.Add(new Vector2(-1, 0));
        }
        if (!state.HasFlag(WallState.UP))
        {
            posibleDirections.Add(new Vector2(0, 1));
        }
        if (!state.HasFlag(WallState.RIGHT))
        {
            posibleDirections.Add(new Vector2(1, 0));
        }
        if (!state.HasFlag(WallState.DOWN))
        {
            posibleDirections.Add(new Vector2(0, -1));
        }
        var debugstring = "";
        for (int i = 0; i < posibleDirections.Count; ++i)
        {
            debugstring = debugstring + ' ' + posibleDirections[i].ToString();
        }


        movement = choiseDirection(posibleDirections);
        //placeMarker(position);
        if (old_movement != movement)
        {
            transform.position = getRenderPosition(position);
            if (movement.x > 0 && eyesDirection != 1)
            {
                eyesDirection = 1;
                eyes.localPosition = new Vector2(1.2f, 1f);
            }
            if (movement.x < 0 && eyesDirection != 0)
            {
                eyesDirection = 0;
                eyes.localPosition = new Vector2(0.7f, 1f);
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

    }

    private Vector3 getMazePosition(Vector3 position)
    {
        if (maze != null)
        {
            int width = maze.GetLength(0);
            int height = maze.GetLength(1);
            return new Vector3(Mathf.Round(position.x + width / 2), Mathf.Round(position.y + height / 2), 0);
        }
        return new Vector3(0, 0);

    }
    private Vector3 getRenderPosition(Vector3 position)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        return new Vector3(-width / 2 + position.x, -height / 2 + position.y, 0);
    }

    void placeMarker(Vector3 position)
    {
        dir_marker.position = getRenderPosition(position);
        var arrow = dir_marker.GetChild(0);
        if (movement.x > 0)
        {
            arrow.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (movement.x < 0)
        {
            arrow.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (movement.y < 0)
        {
            arrow.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (movement.y > 0)
        {
            arrow.rotation = Quaternion.Euler(0, 0, 0);
        }

    }
    void Update()
    {

        var mp = getMazePosition(transform.position);
        var rp = getRenderPosition(mp);
        var delta = transform.position - rp;
        if (Mathf.Abs(delta.x + delta.y) < 0.1f && (mazePosition.x != mp.x || mazePosition.y != mp.y))
        {
            getRoute();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.ToString().Contains("Wall"))
        {
            //movement = new Vector2(0, 0);
            //StartCoroutine(wait());
        }

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.ToString().Contains("Player"))
        {
            var delta = other.transform.position - transform.position;
            if (Mathf.Abs(delta.x + delta.y) < 0.3f)
            {
                PlayerControler playerControler = other.GetComponent<PlayerControler>();
                playerControler.Kill();
            }
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(Random.value / 2);
        getRoute();
    }
}
