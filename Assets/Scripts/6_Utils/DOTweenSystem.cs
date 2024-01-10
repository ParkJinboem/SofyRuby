using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;

public class DOTweenSystem : MonoBehaviour
{
    public bool isAutoStart = false;
    private bool isStart = false;
    private RectTransform rectTransform;

    [Header("Setting")]
    public Ease ease;
    public float duration = 1f;
    public float delay = 0f;
    public bool isRelative = false;
    public bool isLoop;
    public LoopType loopType = LoopType.Restart;
    public int loopCount = 0;

    [Header("Callback")]
    public UnityEvent completeCallback;

    [Header("Animation Type")]
    public AnimationType animationType = AnimationType.None;

    [Header("Local Move")]
    public bool isLocalAnchor;
    public Vector3 localMoveFrom;
    public Vector3 localMoveTo;

    [Header("Move")]
    public Vector3 moveFrom;
    public Vector3 moveTo;

    [Header("Scale")]
    public Vector3 scaleFrom;
    public Vector3 scaleTo;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeFrom;
    public float fadeTo;

    public enum AnimationType
    {
        None,
        LocalMove,
        Move,
        Scale,
        Fade
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(DOTweenSystem))]
    class DOTweenSystemEditor : Editor
    {
        SerializedProperty isAutoStartOption;
        SerializedProperty animationTypeOption;
        SerializedProperty easeOption;
        SerializedProperty durationOption;
        SerializedProperty delayOption;
        SerializedProperty isRelativeOption;
        SerializedProperty isLoopOption;
        SerializedProperty loopTypeOption;
        SerializedProperty loopCountOption;
        SerializedProperty completeCallbackOption;

        SerializedProperty isLocalAnchorOption;
        SerializedProperty localMoveFromOption;
        SerializedProperty localMoveToOption;
        SerializedProperty moveFromOption;
        SerializedProperty moveToOption;
        SerializedProperty scaleFromOption;
        SerializedProperty scaleToOption;
        SerializedProperty fadeCanvasGroupOption;
        SerializedProperty fadeFromOption;
        SerializedProperty fadeToOption;

        private void Awake()
        {
            isAutoStartOption = serializedObject.FindProperty("isAutoStart");
            easeOption = serializedObject.FindProperty("ease");
            durationOption = serializedObject.FindProperty("duration");
            delayOption = serializedObject.FindProperty("delay");
            isRelativeOption = serializedObject.FindProperty("isRelative");
            isLoopOption = serializedObject.FindProperty("isLoop");
            loopTypeOption = serializedObject.FindProperty("loopType");
            loopCountOption = serializedObject.FindProperty("loopCount");
            completeCallbackOption = serializedObject.FindProperty("completeCallback");

            animationTypeOption = serializedObject.FindProperty("animationType");
            isLocalAnchorOption = serializedObject.FindProperty("isLocalAnchor");
            localMoveFromOption = serializedObject.FindProperty("localMoveFrom");
            localMoveToOption = serializedObject.FindProperty("localMoveTo");
            moveFromOption = serializedObject.FindProperty("moveFrom");
            moveToOption = serializedObject.FindProperty("moveTo");
            scaleFromOption = serializedObject.FindProperty("scaleFrom");
            scaleToOption = serializedObject.FindProperty("scaleTo");
            fadeCanvasGroupOption = serializedObject.FindProperty("fadeCanvasGroup");
            fadeFromOption = serializedObject.FindProperty("fadeFrom");
            fadeToOption = serializedObject.FindProperty("fadeTo");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(isAutoStartOption);
            EditorGUILayout.PropertyField(easeOption);
            EditorGUILayout.PropertyField(durationOption);
            EditorGUILayout.PropertyField(delayOption);
            EditorGUILayout.PropertyField(isRelativeOption);
            EditorGUILayout.PropertyField(isLoopOption);
            if (isLoopOption.boolValue)
            {
                EditorGUILayout.PropertyField(loopTypeOption);
                EditorGUILayout.PropertyField(loopCountOption);
            }
            EditorGUILayout.PropertyField(completeCallbackOption);

            EditorGUILayout.PropertyField(animationTypeOption);
            switch ((AnimationType)animationTypeOption.enumValueIndex)
            {
                case AnimationType.LocalMove:
                    EditorGUILayout.PropertyField(isLocalAnchorOption);
                    EditorGUILayout.PropertyField(localMoveFromOption);
                    EditorGUILayout.PropertyField(localMoveToOption);
                    break;
                case AnimationType.Move:
                    EditorGUILayout.PropertyField(moveFromOption);
                    EditorGUILayout.PropertyField(moveToOption);
                    break;
                case AnimationType.Scale:
                    EditorGUILayout.PropertyField(scaleFromOption);
                    EditorGUILayout.PropertyField(scaleToOption);
                    break;
                case AnimationType.Fade:
                    EditorGUILayout.PropertyField(fadeCanvasGroupOption);
                    EditorGUILayout.PropertyField(fadeFromOption);
                    EditorGUILayout.PropertyField(fadeToOption);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (isAutoStart)
        {
            StartAnim();
        }
    }

    private void OnEnable()
    {
        if (isAutoStart)
        {
            StartAnim();
        }
    }

    private void OnDisable()
    {
        isStart = false;
        transform.DOKill();
    }

    private void LocalMove(Vector3 from, Vector3 to, float duration, bool isLoop, LoopType loopType)
    {
        localMoveFrom = from;
        localMoveTo = to;
        this.duration = duration;
        this.isLoop = isLoop;
        this.loopType = loopType;
    }

    public void Move(Vector3 from, Vector3 to, float duration, bool isLoop, LoopType loopType)
    {
        moveFrom = from;
        moveTo = to;
        this.duration = duration;
        this.isLoop = isLoop;
        this.loopType = loopType;
    }

    public void StartAnim()
    {
        if (isStart)
        {
            return;
        }
        isStart = true;

        switch (animationType)
        {
            case AnimationType.LocalMove:
                if (isLocalAnchor)
                {
                    rectTransform.anchoredPosition = localMoveFrom;
                    rectTransform.DOAnchorPos(localMoveTo, duration)
                        .SetEase(ease)
                        .SetRelative(isRelative)
                        .SetDelay(delay)
                        .SetLoops(isLoop ? -1 : loopCount, loopType)
                        .OnComplete(() => { completeCallback?.Invoke(); });
                }
                else
                {
                    transform.localPosition = localMoveFrom;
                    transform.DOLocalMove(localMoveTo, duration)
                        .SetEase(ease)
                        .SetRelative(isRelative)
                        .SetDelay(delay)
                        .SetLoops(isLoop ? -1 : loopCount, loopType)
                        .OnComplete(() => { completeCallback?.Invoke(); });
                }
                break;
            case AnimationType.Move:
                transform.position = moveFrom;
                transform.DOMove(moveTo, duration)
                    .SetEase(ease)
                    .SetRelative(isRelative)
                    .SetDelay(delay)
                    .SetLoops(isLoop ? -1 : loopCount, loopType)
                    .OnComplete(() => { completeCallback?.Invoke(); });
                break;
            case AnimationType.Scale:
                transform.localScale = scaleFrom;
                transform.DOScale(scaleTo, duration)
                    .SetEase(ease)
                    .SetRelative(isRelative)
                    .SetDelay(delay)                    
                    .SetLoops(isLoop ? -1 : loopCount, loopType)
                    .OnComplete(() => { completeCallback?.Invoke(); });
                break;
            case AnimationType.Fade:
                fadeCanvasGroup.alpha = fadeFrom;
                fadeCanvasGroup.DOFade(fadeTo, duration)
                    .SetEase(ease)
                    .SetRelative(isRelative)
                    .SetDelay(delay)
                    .SetLoops(isLoop ? -1 : loopCount, loopType)
                    .OnComplete(() => { completeCallback?.Invoke(); });
                break;
            default:
                break;
        }
    }
}
