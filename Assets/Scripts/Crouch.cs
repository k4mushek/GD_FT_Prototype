using UnityEngine;

public class Crouch : MonoBehaviour
{
    public CharacterController cc;
    [Tooltip("Ѕудем двигать Ё“ќ“ объект. ≈сли не задан, создадим VisualOffset над мешем.")]
    public Transform modelRoot;

    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;
    [Tooltip("Ќасколько визуально опускать модель при приседе")]
    public float crouchOffsetY = -0.35f;
    public float lerpSpeed = 10f;

    float targetHeight;
    float targetY;

    void Awake()
    {
        if (!cc) cc = GetComponent<CharacterController>();

        // если modelRoot не задан Ч создаЄм VisualOffset над первым SkinnedMeshRenderer
        if (modelRoot == null)
        {
            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var meshTr = smr.transform;
                var offsetGO = new GameObject("VisualOffset");
                offsetGO.transform.SetParent(meshTr.parent, false);
                offsetGO.transform.localPosition = meshTr.localPosition;
                offsetGO.transform.localRotation = meshTr.localRotation;
                offsetGO.transform.localScale = meshTr.localScale;

                meshTr.SetParent(offsetGO.transform, true);
                modelRoot = offsetGO.transform;
            }
            else
            {
                // запасной вариант Ч двигаем сам корень, если меша не нашли
                modelRoot = transform;
            }
        }

        targetHeight = standHeight;
        targetY = 0f;
        // на вс€кий: отключи root motion, чтобы аниматор не двигал корень
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.applyRootMotion = false;
    }

    void Update()
    {
        bool crouch = Input.GetKey(KeyCode.LeftControl);

        targetHeight = crouch ? crouchHeight : standHeight;
        targetY = crouch ? crouchOffsetY : 0f;

        // физика-коллайдер
        cc.height = Mathf.Lerp(cc.height, targetHeight, Time.deltaTime * lerpSpeed);
        cc.center = new Vector3(0, cc.height * 0.5f, 0);

        // визуальный оффсет
        var p = modelRoot.localPosition;
        p.y = Mathf.Lerp(p.y, targetY, Time.deltaTime * lerpSpeed);
        modelRoot.localPosition = p;
    }
}