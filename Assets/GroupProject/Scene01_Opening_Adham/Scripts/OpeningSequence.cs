using System.Collections;
using UnityEngine;

namespace GroupProject.Opening
{
    public sealed class OpeningSequence : MonoBehaviour
    {
        [Min(0.1f)] public float fadeSeconds = 3f;
        public Camera headsetCamera;
        public Material fadeMaterial;
        public AudioSource welcome;
        Material instance;
        GameObject overlay;
        IEnumerator Start()
        {
            if (!headsetCamera || !fadeMaterial) yield break;
            overlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
            overlay.name = "Opening fade (runtime)";
            Destroy(overlay.GetComponent<Collider>());
            overlay.transform.SetParent(headsetCamera.transform, false);
            overlay.transform.localPosition = new Vector3(0, 0, headsetCamera.nearClipPlane + 0.02f);
            overlay.transform.localScale = Vector3.one * 4f;
            instance = new Material(fadeMaterial);
            overlay.GetComponent<Renderer>().sharedMaterial = instance;
            instance.SetFloat("_Alpha", 1);
            yield return null;
            float elapsed = 0;
            while (elapsed < fadeSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                instance.SetFloat("_Alpha", 1 - Mathf.SmoothStep(0, 1, elapsed / fadeSeconds));
                yield return null;
            }
            Destroy(overlay);
            if (welcome) welcome.Play();
        }
        void OnDestroy()
        {
            if (overlay) Destroy(overlay);
            if (instance) Destroy(instance);
        }
    }
}
