using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPickups : MonoBehaviour
{
    public static SpwanPickups Instance = null;
    public GameObject longBlastPrefab, moreBombsPrefab, rCBombPrefab, speedBoostPrefab;
    PickupTypes e_pickup;

    private void Awake()
    {
        Instance = this;
    }
    public void SpawnPickup(Vector3 pos)
    {
        StartCoroutine(SpawnPickupAfterSomeDelay(pos));
    }

    IEnumerator SpawnPickupAfterSomeDelay(Vector3 pos)
    {
        yield return new WaitForSeconds(0.2f);

        e_pickup =(PickupTypes) Random.Range(0, System.Enum.GetNames(typeof(PickupTypes)).Length);

        GameObject pickupGO = null;
        switch (e_pickup)
        {
            case PickupTypes.LongBlast:
                pickupGO =Instantiate(longBlastPrefab, pos, Quaternion.identity);
                pickupGO.name = "LongBlast";
                break;
            case PickupTypes.MoreBombs:
                pickupGO = Instantiate(moreBombsPrefab, pos, Quaternion.identity);
                pickupGO.name = "MoreBombs";
                break;
            case PickupTypes.RCBomb:
                pickupGO = Instantiate(rCBombPrefab, pos, Quaternion.identity);
                pickupGO.name = "RCBomb";
                break;
            case PickupTypes.SpeedBoost:
                pickupGO = Instantiate(speedBoostPrefab, pos, Quaternion.identity);
                pickupGO.name = "SpeedBoost";
                break;
        }
    }

}
