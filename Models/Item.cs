using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Models
{
    public enum ItemType { Lost, Found }
    public enum ItemStatus { Pending, Verified, Claimed, Returned }

    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อสิ่งของ")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุรายละเอียดเพิ่มเติม")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุสถานที่พบหรือทำหาย")]
        public string Location { get; set; } = string.Empty;

        [Required]
        public ItemType Type { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Pending;

        public string? ImageUrl { get; set; }

        public DateTime EventDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string StudentId { get; set; } = string.Empty;
    }
}