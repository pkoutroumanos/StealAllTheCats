namespace StealAllTheCats.Dtos
{
    /// <summary>
    /// Data transfer object representing a cat.
    /// </summary>
    public class CatDto
    {    /// <summary>
         /// The unique identifier for the record.
         /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The identifier of the cat image from the Cat API.
        /// </summary>
        public string CatId { get; set; }
        /// <summary>
        /// The width of the cat image.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of the cat image.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The creation timestamp of the record.
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// List of tags associated with the cat.
        /// </summary
        public List<string> Tags { get; set; } = new();
    }
}
