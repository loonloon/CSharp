using System.ComponentModel.DataAnnotations;

namespace ICOCrawler.DataAccess.Sqlite.Model
{
    public class InitialCoinOffering
    {
        [Key]
        public int No { get; set; }
        public string Label { get; set; }
        public string Hyperlink { get; set; }
        public bool IsVerified { get; set; }
        public string CreatedDate { get; set; }
        public int InitialCoinOfferingSourceId { get; set; }
    }
}
