namespace CreationEditor.Notification;

public sealed record NotificationItem(Guid ID, string? LoadText = null, float? LoadProgress = null);
