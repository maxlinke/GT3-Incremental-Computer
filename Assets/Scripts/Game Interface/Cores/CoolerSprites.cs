using System.Collections.Generic;
using UnityEngine;

public class CoolerSprites : ScriptableObject {

    [SerializeField] Sprite[] m_fanSprites;
    [SerializeField] Sprite[] m_iceSprites;

    public IReadOnlyList<Sprite> fanSprites => m_fanSprites;
    public IReadOnlyList<Sprite> iceSprites => m_iceSprites;

}
