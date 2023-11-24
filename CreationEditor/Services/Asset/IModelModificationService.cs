namespace CreationEditor.Services.Asset;

public interface IModelModificationService {
    /// <summary>
    /// Remaps links in a file based on a predicate to another link
    /// </summary>
    /// <param name="file">Full path to the file to remap links for</param>
    /// <param name="shouldReplace">Function that decides if a link should be replaced with the new link</param>
    /// <param name="newLink">New link that replaces old links</param>
    void RemapLinks(string file, Func<string, bool> shouldReplace, string newLink);
}
