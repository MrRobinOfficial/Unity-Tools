using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TriggerZone
{
    [RequireComponent(typeof(BoxCollider))]
    [ExecuteAlways]
    public class TriggerVolume : MonoBehaviour
    {
        public enum TriggerState : byte
        {
            EnterAndExit,
            Stay,
        }

        public event UnityAction OnEnterEvent;
        public event UnityAction OnExitEvent;

        [Header("General Settings")]
        [SerializeField] TriggerState m_State = TriggerState.EnterAndExit;
        [SerializeField] bool m_DestroyAfter = false;
        [SerializeField] LayerMask m_CollideWith = 1;
        [SerializeField, Tag] string m_IgnoreTag = string.Empty;

        [Header("Events"), Space]
        [SerializeField] UnityEvent m_OnEnterEvent = default;
        [SerializeField] UnityEvent m_OnExitEvent = default;

        public BoxCollider Collider => collider;

        [HideInInspector] private new BoxCollider collider;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (collider == null)
            {
                collider = GetComponent<BoxCollider>();
                return;
            }

            const float ALPHA = 0.8f;

            var bounds = collider.bounds;

            var innerColor = (Color)new Color32(32, 32, 32, 255);
            innerColor.a = ALPHA;
            Gizmos.color = innerColor;

            Gizmos.DrawCube(transform.position, bounds.size);

            var outerColor = Color.green;
            outerColor.a = ALPHA;
            Gizmos.color = outerColor;

            Gizmos.DrawWireCube(transform.position, bounds.size);
        }

        [UnityEditor.MenuItem("GameObject/Tools/Create Trigger Volume", priority = -100150)]
        private static void CreateVolume(MenuCommand menuCommand)
        {
            var obj = new GameObject(name: "Custom Trigger Volume");
            obj.AddComponent<TriggerVolume>();

            var view = UnityEditor.SceneView.lastActiveSceneView.camera;
            var pos = view.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

            obj.transform.position = pos;

            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeObject = obj;
        }
#endif

        private void Start()
        {
            if (collider == null)
            {
                if (!TryGetComponent(out collider))
                    collider = gameObject.AddComponent<BoxCollider>();
            }

            collider.hideFlags = HideFlags.NotEditable;
            collider.isTrigger = true;
            collider.size = Vector3.one;
        }

        private void OnEnable() => hasTriggerd = false;

        private bool hasTriggerd = false;
        private GameObject prevObj = null;

        private void OnTriggerEnter(Collider other)
        {
            if (m_State != TriggerState.EnterAndExit)
                return;

            if (hasTriggerd && m_DestroyAfter)
                return;

            var rootObj = other.transform.root.gameObject;

            if (!string.IsNullOrEmpty(m_IgnoreTag) && rootObj.CompareTag(m_IgnoreTag))
                return;

            if (!HasLayer(rootObj, m_CollideWith))
                return;

            prevObj = other.gameObject;
            m_OnEnterEvent?.Invoke();
            OnEnterEvent?.Invoke();
            hasTriggerd = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (m_State != TriggerState.Stay)
                return;

            var rootObj = other.transform.root.gameObject;

            if (!string.IsNullOrEmpty(m_IgnoreTag) && rootObj.CompareTag(m_IgnoreTag))
                return;

            if (!HasLayer(rootObj, m_CollideWith))
                return;

            prevObj = other.gameObject;
            m_OnEnterEvent?.Invoke();
            OnEnterEvent?.Invoke();
            hasTriggerd = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (prevObj != other.gameObject)
                return;

            prevObj = null;
            m_OnExitEvent?.Invoke();
            OnExitEvent?.Invoke();

            if (m_DestroyAfter)
                Destroy(gameObject);
        }

        private bool HasLayer(GameObject obj, LayerMask layerMask) => layerMask == (layerMask | (1 << obj.layer));

        public void DebugMessage(string msg) => Debug.Log(msg, this);
    } 
}