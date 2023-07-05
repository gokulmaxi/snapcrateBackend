using System.ComponentModel.DataAnnotations;

namespace snapcrateBackend.Model
{
    public class ImageModel
    {
        [Key]
        public int imageId { get; set; }
        public string name { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public FolderModel folder { get; set; }
    }
}
