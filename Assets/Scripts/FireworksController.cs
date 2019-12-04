using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksController : MonoBehaviour {

    public GameObject[] fireworks;

	// Use this for initialization
	void Start () {
        StartCoroutine("ShootFireWorks");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator ShootFireWorks() {
        while (true) {
            fireworks[Random.Range(0, 3)].GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(Random.Range(.5f, 1f));
        }
    }
}
