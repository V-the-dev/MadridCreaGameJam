using UnityEngine;


public class StairsZone : MonoBehaviour
{
    public float angleZ;
    [SerializeField]private float climbMult = 0.8f;

    public Vector2 newVec;

    private void Awake()
    {
        angleZ = transform.eulerAngles.z;
        if (angleZ > 180)
            angleZ = -
        Mathf.Abs(angleZ);

        newVec = new Vector2(Mathf.Cos(angleZ * Mathf.Deg2Rad), Mathf.Sin(angleZ * Mathf.Deg2Rad));

        //newVec.Normalize();
        //newVec *= climbMult;

    }


}
