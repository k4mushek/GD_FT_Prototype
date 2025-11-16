using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public enum MissionState
    {
        Infiltration,   // до того, как вз€ли Power Cell
        Escape,         // бежим по крышам
        Completed
    }

    [Header("UI")]
    public TMP_Text mainObjectiveText;
    public TMP_Text optionalObjectiveText;

    [Header("Spawners")]
    public EnemySpawner streetSpawner;    // волна на улице после лута
    public EnemySpawner roofSpawner;      // доп. враги на крышах по спотам WatcherТа

    [Header("Objective Texts")]
    [TextArea]
    public string mainStartText =
        "Main: Infiltrate the district and steal the power cell from the relay station.";
    [TextArea]
    public string mainEscapeText =
        "Main: Reach the extraction point on the rooftops.";
    [TextArea]
    public string optionalUnknownText =
        "Optional: ???";
    [TextArea]
    public string optionalKnownText =
        "Optional: Rescue the trapped survivor.";
    [TextArea]
    public string optionalCompletedText =
        "Optional: Survivor rescued.";

    public MissionState State { get; private set; } = MissionState.Infiltration;
    bool optionalDiscovered;
    bool optionalCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // стартовые задачи
        if (mainObjectiveText) mainObjectiveText.text = mainStartText;
        if (optionalObjectiveText) optionalObjectiveText.text = optionalUnknownText;
    }

    // ==== API ====

    // »грок вошЄл в зону вокруг выжившего
    public void DiscoverOptional()
    {
        if (optionalDiscovered || optionalCompleted) return;
        optionalDiscovered = true;
        if (optionalObjectiveText) optionalObjectiveText.text = optionalKnownText;
    }

    // »грок реально спас/освободил выжившего
    public void CompleteOptional()
    {
        if (optionalCompleted) return;
        optionalCompleted = true;
        if (optionalObjectiveText) optionalObjectiveText.text = optionalCompletedText;
        // сюда можно добавить +1 к морали, к кол-ву людей в базе и т.п.
    }

    // ¬з€ли Power Cell в Relay Station
    public void CompleteMainObjective()
    {
        if (State != MissionState.Infiltration) return;
        State = MissionState.Escape;

        if (mainObjectiveText) mainObjectiveText.text = mainEscapeText;

        // поднимаем ад на улице
        if (streetSpawner) streetSpawner.SpawnWave();
    }

    // »грок добралс€ до финальной точки эвакуации на крышах
    public void ReachedExtraction()
    {
        if (State != MissionState.Escape) return;
        State = MissionState.Completed;

        if (mainObjectiveText) mainObjectiveText.text = "Mission complete. Return to base.";
        // можно загрузить сцену базы / включить финальный экран и т.п.
    }

    // ¬ызываетс€ WatcherТом, когда тот спалил игрока на крыше
    public void OnRoofSpotted(Vector3 spotPosition)
    {
        if (State != MissionState.Escape) return;
        if (!roofSpawner) return;

        roofSpawner.SpawnFewAt(spotPosition);
    }
}
