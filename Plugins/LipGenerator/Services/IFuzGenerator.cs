namespace LipGenerator.Services;

public interface IFuzGeneratorArgs;

public interface IFuzGenerator {
    void GenerateFuz(string srcDir, string dstDir, string audioExt);
}
