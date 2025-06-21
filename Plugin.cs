using BepInEx;
using GorillaLocomotion;
using UnityEngine;

namespace MonkeClick
{
    [BepInPlugin("baggz.monkeclick", "MonkeClick", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static LineRenderer lineRenderer;
        public static bool isEnabled = false;

        void OnEnable()
        {
            isEnabled = true;
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
            Debug.Log("MonkeClick enabled.");
        }

        void OnDisable()
        {
            isEnabled = false;
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
                lineRenderer = null;
            }
            Debug.Log("MonkeClick disabled.");
        }

        void OnGameInitialized()
        {
            if (!isEnabled) return;

            GameObject lineObj = new GameObject("RaycastLine");
            lineRenderer = lineObj.AddComponent<LineRenderer>();

            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.enabled = false;
        }

        void FixedUpdate()
        {
            if (!isEnabled || lineRenderer == null) return;

            lineRenderer.SetPosition(0, GTPlayer.Instance.rightControllerTransform.position);

            if (ControllerInputPoller.instance.rightGrab)
            {
                lineRenderer.enabled = true;

                var hits = Physics.RaycastAll(
                    GTPlayer.Instance.rightControllerTransform.position,
                    GTPlayer.Instance.rightControllerTransform.forward,
                    Mathf.Infinity,
                    ~(1 << LayerMask.NameToLayer("Gorilla Boundary")));

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (var hit in hits)
                {
                    lineRenderer.SetPosition(1, hit.point);

                    if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f &&
                        hit.transform.gameObject.layer == 18)
                    {
                        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                        break;
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
