using UnityEngine;
using UnityEngine.Events;

public class WatcherController : MonoBehaviour
{
    [System.Serializable]
    public class WatcherPattern
    {
        public string name;
        [Tooltip("Углы поворота относительно стартового Y, в градусах")]
        public float[] localYawAngles;
        [Tooltip("Скорость поворота в град/сек")]
        public float rotationSpeed = 20f;
        [Tooltip("Пауза в секундах в каждой точке")]
        public float holdTime = 1.5f;
    }

    [Header("Setup")]
    public Transform pivot;          // то, что крутится (голова/спотлайт)
    public Light spotLight;          // сам прожектор (не обязательно)
    public Transform player;         // цель для детекта
    public LayerMask obstacleMask;   // стены/здания

    [Header("Patterns")]
    public WatcherPattern[] patterns;
    public int startPatternIndex = 0;
    public bool randomizePatterns = true;

    [Header("Vision")]
    public float viewDistance = 100f;
    [Range(0f, 90f)]
    public float viewAngle = 25f; // половина конуса
    public float alertCooldown = 2f;

    [Header("Events")]
    public UnityEvent<Vector3> OnPlayerSpotted; // можно повесить менеджер охраны

    float _baseYaw;
    int _currentPatternIndex;
    int _currentAngleIndex;
    float _holdTimer;
    float _alertTimer;

    enum State { Rotating, Holding }
    State _state = State.Rotating;

    void Start()
    {
        if (!pivot) pivot = transform;
        _baseYaw = pivot.eulerAngles.y;

        if (patterns == null || patterns.Length == 0)
        {
            enabled = false;
            Debug.LogWarning("WatcherController: нет паттернов, скрипт выключен", this);
            return;
        }

        SetPattern(Mathf.Clamp(startPatternIndex, 0, patterns.Length - 1));
    }

    void SetPattern(int idx)
    {
        _currentPatternIndex = idx;
        _currentAngleIndex = 0;
        _state = State.Rotating;
    }

    void Update()
    {
        var pattern = patterns[_currentPatternIndex];

        // таймер алерта, чтобы не спамить
        if (_alertTimer > 0f)
            _alertTimer -= Time.deltaTime;

        switch (_state)
        {
            case State.Rotating:
                RotateTowardsCurrentAngle(pattern);
                break;

            case State.Holding:
                _holdTimer -= Time.deltaTime;
                if (_holdTimer <= 0f)
                    NextAngleOrPattern(pattern);
                break;
        }

        CheckVision();
    }

    void RotateTowardsCurrentAngle(WatcherPattern pattern)
    {
        if (pattern.localYawAngles == null || pattern.localYawAngles.Length == 0)
            return;

        float targetYaw = _baseYaw + pattern.localYawAngles[_currentAngleIndex];
        Vector3 euler = pivot.eulerAngles;
        float newYaw = Mathf.MoveTowardsAngle(euler.y, targetYaw, pattern.rotationSpeed * Time.deltaTime);
        pivot.rotation = Quaternion.Euler(euler.x, newYaw, euler.z);

        float delta = Mathf.DeltaAngle(newYaw, targetYaw);
        if (Mathf.Abs(delta) < 0.5f)
        {
            _state = State.Holding;
            _holdTimer = pattern.holdTime;
        }
    }

    void NextAngleOrPattern(WatcherPattern pattern)
    {
        _currentAngleIndex++;

        if (_currentAngleIndex >= pattern.localYawAngles.Length)
        {
            // конец паттерна
            if (randomizePatterns && patterns.Length > 1)
            {
                int next = _currentPatternIndex;
                // выбираем другой паттерн
                while (next == _currentPatternIndex)
                    next = Random.Range(0, patterns.Length);
                SetPattern(next);
            }
            else
            {
                // просто зациклить текущий
                _currentAngleIndex = 0;
            }

            _state = State.Rotating;
        }
        else
        {
            _state = State.Rotating;
        }
    }

    void CheckVision()
    {
        if (!player) return;

        Vector3 toPlayer = player.position - pivot.position;
        float dist = toPlayer.magnitude;
        if (dist > viewDistance) return;

        float angle = Vector3.Angle(pivot.forward, toPlayer);
        if (angle > viewAngle) return;

        // проверка на стены
        if (Physics.Raycast(pivot.position, toPlayer.normalized, dist, obstacleMask))
            return;

        if (_alertTimer <= 0f)
        {
            _alertTimer = alertCooldown;
            // здесь можно звать менеджер, ботов, что угодно
            Debug.Log("WATCHER SPOTTED PLAYER at " + player.position, this);
            OnPlayerSpotted?.Invoke(player.position);
        }
    }

    // Гизмы для дебага в сцене
    void OnDrawGizmosSelected()
    {
        if (!pivot) pivot = transform;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(pivot.position, viewDistance);

        // конус зрения
        Vector3 left = Quaternion.Euler(0, -viewAngle, 0) * pivot.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle, 0) * pivot.forward;
        Gizmos.DrawRay(pivot.position, left * viewDistance);
        Gizmos.DrawRay(pivot.position, right * viewDistance);
    }
}
