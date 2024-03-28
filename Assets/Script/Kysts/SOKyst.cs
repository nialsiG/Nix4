using UnityEngine;

[CreateAssetMenu(fileName = "Kyst", menuName = "Scriptable object/Kyst")]
public class SOKyst : ScriptableObject
{
    public Sprite visibleSprite, hiddenSprite, histoSprite, flagSprite;
    public string kystName, kystDescription;
    public int size;
    public bool isTumor;
    [SerializeField] private GameObject _prefab;

    public void SpawnKyst(Transform parent)
    {
        GameObject kyst = Instantiate(_prefab, parent);
        kyst.GetComponent<Kyst>().SOKyst = this;

    }
}
