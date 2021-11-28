using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(0, 100)]
    private int width = 50;
    [SerializeField]
    [Range(0, 100)]
    private int height = 50;

    [SerializeField]
    [Range(0, 5000)]
    private int seed = 0;

    [SerializeField]
    [Range(0, 100)]
    private int fullness = 100;

    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform playerPrefab = null;


    [SerializeField]
    private Transform coinPrefab = null;

    [SerializeField]
    [Range(1, 1000)]
    private int coinsCount = 10;

    [SerializeField]
    private Transform enemyPrefab = null;

    [SerializeField]
    [Range(1, 400)]
    private int enemysCount = 3;

     void Start()
    {
        var maze = MazeGenerator.Generate(width, height, seed, fullness);
        Draw(maze);
        PlaceCoins();
        PlaceEnemys(maze);


        var player = Instantiate(playerPrefab, transform);
        var playerControler = player.GetComponent<PlayerControler>();
        playerControler.init(maze);

        var cam_controler = Camera.main.GetComponent<CameraControler>();
        cam_controler.setTarget(player);
        if (cam_controler.enabled)
        {
            var pos = new Vector3(player.position.x, player.position.y, -40f);
            Camera.main.transform.position = pos;
        }

    }

    private Transform[] coins;
    public void setCoinRandomPosition(Transform coin)
    {
        coin.tag = "noCoin";
        var randomPosition = new Vector2(Mathf.Floor(Random.value * width), Mathf.Floor(Random.value * height));
        var position = new Vector3(-width / 2 + randomPosition.x + 0.5f, -height / 2 + randomPosition.y + 0.5f, 0);
        coin.position = position;
        var coins = GameObject.FindGameObjectsWithTag("Coin");
        for (int i = 0; i < coins.Length; ++i)
        {
            if (coins[i].transform.position == coin.position)
            {
                setCoinRandomPosition(coin);
                break;
            }
        }
        coin.tag = "Coin";
    }

    private void PlaceEnemys(WallState[,] maze)
    {
        for (int i = 0; i < enemysCount; ++i)
        {
            var enemy = Instantiate(enemyPrefab, transform);
            var randomPosition = new Vector2(Mathf.Floor(Random.value * width), Mathf.Floor(Random.value * height));
            var position = new Vector3(-width / 2 + randomPosition.x, -height / 2 + randomPosition.y, 0);
            EnemyControler enemyControler = enemy.GetComponent<EnemyControler>();
            enemy.position = position;
            enemyControler.init(maze);
        }
    }
    private void PlaceCoins()
    {
        for (int i = 0; i < coinsCount; ++i)
        {
            var coin = Instantiate(coinPrefab, transform);
            setCoinRandomPosition(coin);
        }
    }

    private void PlacePlayer(Vector2 position)
    {
        var player = Instantiate(playerPrefab, transform);
        var pos = new Vector3(-width / 2 + position.x + size / 2, -height / 2 + position.y + size / 2, 0);
        player.position = pos;
        var cam_controler = Camera.main.GetComponent<CameraControler>();
        cam_controler.setTarget(player);
    }

    private void Draw(WallState[,] maze)
    {

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, -height / 2 + j, 0);
                //var floor = Instantiate(floorPrefab, transform);
                //floor.position=position + new Vector3(size/2, size/2, 0);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(size / 2, size, 0);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(0, size / 2, 0);
                    leftWall.eulerAngles = new Vector3(0, 0, 90);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(size, size / 2, 0);
                        rightWall.eulerAngles = new Vector3(0, 0, 90);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(size / 2, 0, 0);
                    }
                }
            }

        }


    }


}
