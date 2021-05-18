using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int")]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public int InitialValue;

    [SerializeField]
    private int currentValue;

    /// <summary>
    /// Runtime Value
    /// </summary>
    [System.NonSerialized]
    private int Value;

    public int GetValue() {
        return Value;
    }

    public void OnAfterDeserialize()
    {
        Value = InitialValue;
    }

    public void OnBeforeSerialize() { currentValue = Value;  }

    public void SetValue(int value)
    {
        Value = value;
        currentValue = Value;
    }

    public void SetValue(IntVariable value)
    {
        Value = value.Value;
        currentValue = Value;
    }

    public void Add(int amount)
    {
        Value += amount;
        currentValue = Value;
    }

    public void Add(IntVariable amount)
    {
        Value += amount.Value;
        currentValue = Value;
    }
}
