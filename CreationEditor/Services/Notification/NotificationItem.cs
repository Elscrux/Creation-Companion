namespace CreationEditor.Services.Notification;

public sealed record NotificationItem(Guid Id, string? LoadText = null, float? LoadProgress = null) {
    public bool IsLive => LoadText is not null;
    public bool IsDone => LoadText is null;
}
