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
                Instantiate(UIManager.main.GetSuccessCheckIcon(), transform.position, Quaternion.identity);
                Instantiate(UIManager.main.GetSuccessCheckEffect(), transform.position, Quaternion.identity);

                if (UIManager.main != null)
                {
                    Instantiate(UIManager.main.GetRewardObject(), transform.position, Quaternion.identity);

                    // GameObject rewardObject = Instantiate(reward, worldPosition, Quaternion.identity);
                    // IconSuccess rewardComponent = rewardObject.GetComponent<IconSuccess>();
                    // rewardComponent.SetTarget(UIDifferences[currentClick], true);
                }

                if (linkedObject != null)
                {
                    Instantiate(UIManager.main.GetSuccessCheckIcon(), linkedObject.transform.position, Quaternion.identity);
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
}
