namespace CreationEditor.Services.UserInput;

/// <summary>
/// Opaque abstraction over user interaction: asks the user for a text value without the caller
/// knowing how (e.g. a dialog in the UI, a console prompt, or a stub in tests).
/// </summary>
public interface IUserInputService {
    /// <summary>
    /// Asks the user to enter a text value.
    /// </summary>
    /// <param name="title">Title of the prompt (e.g. dialog header).</param>
    /// <param name="message">Message shown to the user.</param>
    /// <param name="defaultValue">Optional pre-filled value.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The entered text, or null if the user canceled the prompt.</returns>
    Task<string?> RequestTextAsync(string title, string message, string? defaultValue = null, CancellationToken token = default);
}
