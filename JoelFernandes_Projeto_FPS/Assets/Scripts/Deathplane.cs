using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deathplane : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() != null)
        {
            SceneManager.LoadScene(2);
        }
    }
}
