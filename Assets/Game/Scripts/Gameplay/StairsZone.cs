using Unity.VisualScripting;
using UnityEngine;


public class StairsZone : MonoBehaviour
{
    public float angleZ;

    public Vector2 newVec;

    private void Awake()
    {
        angleZ = transform.eulerAngles.z;
        if (angleZ > 180)
            angleZ -=180;


        newVec = new Vector2(Mathf.Abs(Mathf.Cos(angleZ * Mathf.Deg2Rad)), Mathf.Abs(Mathf.Sin(angleZ * Mathf.Deg2Rad)));

    }


}
