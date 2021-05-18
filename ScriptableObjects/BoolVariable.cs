using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public bool InitialValue;

    [SerializeField]
    private bool currentValue;

    /// <summary>
    /// Runtime Value
    /// </summary>
    [System.NonSerialized]
    private bool Value;

    public bool GetValue()
    {
        return Value;
    }

    public void OnAfterDeserialize()
    {
        Value = InitialValue;
    }

    public void OnBeforeSerialize() { currentValue = Value; }

    public void SetValue(bool value)
    {
        Value = value;
        currentValue = Value;
    }

    public void SetValue(BoolVariable value)
    {
        Value = value.Value;
        currentValue = Value;
    }
}
