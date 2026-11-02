using Microsoft.EntityFrameworkCore;

namespace CRDUProject
{
    internal class NotesManager : INotesManager
    {
        public async Task CreatedNote(int userid, string nameNotes, string notes)
        {
            using (DBContext dbContext = new DBContext())
            {
                if (await dbContext.Notes.AnyAsync(u => u.User_Id == userid && u.Name == nameNotes))
                    return;

                var elementNote = new Notes
                {
                    User_Id = userid,
                    Name = nameNotes,
                    Data = notes
                };

                await dbContext.Notes.AddAsync(elementNote);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<string?> ReadNote(int userId, string name)
        {
            using (DBContext dbContext = new DBContext())
            {
                var elementNote = await dbContext.Notes.FirstOrDefaultAsync(u => u.Name == name && u.User_Id == userId);
                return elementNote?.Data;
            }
        }

        public async Task UpdateNote(int userId, string name, string note)
        {
            using (DBContext dbContext = new DBContext())
            {
                var elementNote = await dbContext.Notes.FirstOrDefaultAsync(u => u.Name == name && u.User_Id == userId);
                elementNote?.Data = note;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteNote(int userId, string name)
        {
            using (DBContext dbContext = new DBContext())
            {
                await dbContext.Notes.Where(u => u.Name == name && u.User_Id == userId).ExecuteDeleteAsync();
            }
        }
    }
}