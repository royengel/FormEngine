namespace FormEngine.Interfaces
{
    public interface IFormBuilder
    {
        IFormPage AddPage(PageSize pageSize);
        bool Finalize();
    }
}
