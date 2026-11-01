namespace CRDUProject
{
    internal interface INotesManager
    {
        Task CreatedNote(int userid, string nameNotes, string notes);
        Task<string?> ReadNote(int userId, string name);
        Task UpdateNote(int userId, string name, string note);
        Task DeleteNote(int userId, string name);
    }
}