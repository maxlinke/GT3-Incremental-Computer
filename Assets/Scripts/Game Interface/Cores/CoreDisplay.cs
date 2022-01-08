using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cores;

public class CoreDisplay : MonoBehaviour {

    public const int NUMBER_OF_CORES_HORIZONTAL = 4;
    public const int NUMBER_OF_CORES_VERTICAL = 2;
    public const int NUMBER_OF_CORES = NUMBER_OF_CORES_HORIZONTAL * NUMBER_OF_CORES_VERTICAL;

    [SerializeField, RedIfEmpty] Canvas m_canvas;
    [SerializeField, RedIfEmpty] CoreView m_coreTemplate;
    [SerializeField, RedIfEmpty] CoreInfoDisplay m_infoDisplay;

    public static CoreDisplay instance { get; private set; }

    public bool visible { 
        get => m_canvas.enabled;
        set => m_canvas.enabled = value;
    }

    CoreView[] m_cores;

    public void Initialize () {
        instance = this;
        SpawnCores();
        m_infoDisplay.Initialize(this);
    }

    public CoreView GetCore (int index) => m_cores[index];

    public IEnumerable<CoreView> Cores => m_cores;

    void SpawnCores () {
        m_coreTemplate.SetGOActive(false);
        m_cores = new CoreView[NUMBER_OF_CORES];
        SpawnThingsAndAlignLikeCores((i) => {
            var newCore = Instantiate(m_coreTemplate, m_coreTemplate.transform.parent);
            newCore.SetGOActive(true);
            newCore.Initialize(i);
            m_cores[i] = newCore;
            return newCore.rectTransform;
        });
    }

    public static void SpawnThingsAndAlignLikeCores (System.Func<int, RectTransform> spawnThing) {
        int i=0;
        var pHeight = 1f / NUMBER_OF_CORES_VERTICAL;
        var pWidth = 1f / NUMBER_OF_CORES_HORIZONTAL;
        for(int y=0; y<NUMBER_OF_CORES_VERTICAL; y++){
            var yMax = 1f - (y * pHeight);
            var yMin = yMax - pHeight;
            for(int x=0; x<NUMBER_OF_CORES_HORIZONTAL; x++){
                var xMin = x * pWidth;
                var xMax = xMin + pWidth;
                var newThing = spawnThing(i);
                i++;
                newThing.anchorMin = new Vector2(xMin, yMin);
                newThing.anchorMax = new Vector2(xMax, yMax);
                newThing.sizeDelta = Vector2.zero;
                newThing.anchoredPosition = Vector2.zero;
            }
        }
    }

}
