

using UnityEngine;

public class GameData 
{

    public float[] playerPosition;

    public GameData( Vector3 position)
    {

        
        playerPosition = new float[] { position.x, position.y, position.z };
        
    }
}
