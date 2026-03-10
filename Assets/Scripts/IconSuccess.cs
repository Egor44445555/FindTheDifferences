using UnityEngine;

public class IconSuccess : MonoBehaviour
{
    [SerializeField] float moveSpeed = 80f;
    [SerializeField] GameObject star;

    UIDifference target;
    bool goToTarget = false;
    float timer = 0f;
    bool success = true;

    void Update()
    {
        if (target != null)
        {
            timer += Time.deltaTime;

            if (timer > 0.5f)
            {
                goToTarget = true;
            }
        }

        if (goToTarget)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(target.transform.position);
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                        
            if (transform.position == targetPosition)
            {
                target.CatchHandler(success);
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(UIDifference _target, bool _success = false)
    {
        target = _target;
        success = _success;

        if (star != null)
        {
            for (var i = 10; i > 0; i--)
            {
                GameObject starObj = Instantiate(star, transform.position, Quaternion.identity);
                starObj.GetComponent<Star>().SetTarget(_target);
            }
        }
    }
}
