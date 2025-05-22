using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShot : MonoBehaviour
{
  
    private void OnCollisionEnter(Collision collision)
    {
       
        PlayerCharacter character = collision.gameObject.GetComponent<PlayerCharacter>();
        if (character != null)
        {
            character.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}
