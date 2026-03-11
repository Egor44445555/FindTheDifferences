using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] float moveSpeed = 80f;
    [SerializeField] float startMoveSpeed = 100f;
    [SerializeField] float dropOffsetRange = 0.3f;

    UIDifference target;
    bool goToTarget = false;
    float timer = 0f;
    float randomTimerGoToTarget = 0f;
    Vector3 randomOffset;
    Vector3 startPosition;
    AudioSource audioSource;
    bool isDestroy = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        randomOffset = new Vector3(Random.Range(-dropOffsetRange, dropOffsetRange), Random.Range(-dropOffsetRange, dropOffsetRange), 0f);
        startPosition = transform.position + randomOffset;
        randomTimerGoToTarget = Random.Range(1f, 1.4f);
    }

    void Update()
    {
        if (target != null)
        {
            timer += Time.deltaTime;

            if (timer > randomTimerGoToTarget)
            {
                goToTarget = true;
            }
        }

        if (goToTarget)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(target.transform.position);
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                        
            if (!isDestroy && transform.position == targetPosition && UIManager.main != null && UIManager.main.IsSoundsActive())
            {
                audioSource.Play();
                isDestroy = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, startMoveSpeed * Time.deltaTime);
        }

        if (isDestroy && !audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(UIDifference _target)
    {
        target = _target;
    }
}
