namespace CreationEditor.Settings;

public interface ISetting {
    public string Name { get; }
    
    public Type? Parent { get; }
    
    public List<ISetting> Children { get; }
}
