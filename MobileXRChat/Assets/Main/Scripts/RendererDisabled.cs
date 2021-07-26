using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDVR
{
    public class RendererDisabled : MonoBehaviour
    {
        List<Renderer> renderersList_ = new List<Renderer>();

        void OnEnable()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.enabled = false;
                rend.forceRenderingOff = true;

                if (rend.enabled)
                {
                    renderersList_.Add(rend);
                }
            }
        }

        void OnDisable()
        {
            foreach (Renderer rend in renderersList_)
            {
                rend.enabled = true;
                rend.forceRenderingOff = false;
            }

            renderersList_.Clear();
        }
    }
}
