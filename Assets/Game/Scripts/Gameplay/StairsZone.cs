using UnityEngine;


public class StairsZone : MonoBehaviour
{
    [Tooltip("Velocidad relativa del movimiento cuando estás en la escalera")]
    public float climbMultiplier = 0.8f;

    private Vector2 stairsDirection;

    public Vector2 movementMults = new Vector2(0.8f, 0.8f);

    private float angle;

    private void Awake()
    {

        angle = transform.eulerAngles.z;// * Mathf.Deg2Rad;
        //stairsDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

}
