using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRDUProject
{
    [Table("notes")]
    public class Notes
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }


        [Required]
        public int User_Id { get; set; }
        [ForeignKey(nameof(User_Id))]
        public Users users { get; set; }
    }
}
