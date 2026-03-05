using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IModelService {
    bool HasCollision(DataSourceFileLink fileLink);
}
