using UnityEngine;

public class Difference : MonoBehaviour
{
    [SerializeField] GameObject linkedObject;

    bool activeDifference = false;

    public void Catch()
    {
        if (!activeDifference)
        {
            if (UIManager.main != null)
            {
                Instantiate(UIManager.main.GetSuccessMarker(), transform.position, Quaternion.identity, transform);
                Instantiate(UIManager.main.GetSuccessCheckEffect(), transform.position, Quaternion.identity);

                if (UIManager.main != null)
                {
                    GameObject rewardObject = Instantiate(UIManager.main.GetRewardObject(), transform.position, Quaternion.identity);
                    IconSuccess rewardComponent = rewardObject.GetComponent<IconSuccess>();

                    if (UIManager.main != null)
                    {
                        rewardComponent.SetTarget(UIManager.main.GetUIDifferences()[UIManager.main.GetCurrentClick()], true);
                    }
                }

                if (linkedObject != null)
                {
                    Instantiate(UIManager.main.GetSuccessMarker(), linkedObject.transform.position, Quaternion.identity, linkedObject.transform);
                    Instantiate(UIManager.main.GetSuccessCheckEffect(), linkedObject.transform.position, Quaternion.identity);
                    linkedObject.GetComponent<Difference>().CatchActive();
                }                
            }
        }        
        
        activeDifference = true;
    }


    public void CatchActive()
    {
        activeDifference = true;
    }

    public bool IsActive()
    {
        return activeDifference;
    }

    public GameObject GetLinkedObject()
    {
        return linkedObject;
    }
}
