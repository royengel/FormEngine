namespace FormEngine.Interfaces
{
    public interface IResources
    {
        byte[] Get(string name);
        string GetText(string name);
    }
}
