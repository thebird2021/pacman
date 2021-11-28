using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinControler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.ToString().Contains("Player")){
            PlayerControler playerControler=other.GetComponent<PlayerControler>(); 
            playerControler.AddScore();
            transform.position=new Vector3(-1000,-1000,0);
            StartCoroutine(wait());

        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        gameObject.GetComponentInParent<MazeRenderer>().setCoinRandomPosition(this.transform);
    }

}
