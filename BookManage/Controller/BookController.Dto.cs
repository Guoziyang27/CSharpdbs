using System;
using System.ComponentModel.DataAnnotations;

namespace BookManage.Controller
{
    public class AuthenticateReqDto
    {
        [Required] [StringLength(20)] public string CardId { get; set; }
        [Required] [StringLength(20)] public string Password { get; set; }
    }
    public class BookGetListReqDto
    {
        public SortReqDto Sort { get; set; }
        public RangeReqDto Range { get; set; }
        [Required] public BookQueryReqDto Filter { get; set; }
    }

    public class SortReqDto
    {
        [Required] [StringLength(20)] public string Field { get; set; }
        [Required] [StringLength(20)] public string Order { get; set; }
    }
    
    public class RangeReqDto
    {
        [Required] [StringLength(20)] public int Lower { get; set; }
        [Required] [StringLength(20)] public int Upper { get; set; }
    }
    
    public class BookReqDto
    {
        [Required]
        [StringLength(20)]
        public string Class { get; set; }
        [Required]
        [StringLength(20)]
        public string Title { get; set; }
        [Required]
        [StringLength(20)]
        public string Publish { get; set; }
        [Required]
        public int Pubyear { get; set; }
        [Required]
        [StringLength(20)]
        public string Author { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Number { get; set; }
    }
    
    public class BookQueryReqDto
    {
        [Required]
        [StringLength(20)]
        public string Class { get; set; }
        [Required]
        [StringLength(20)]
        public string Title { get; set; }
        [Required]
        [StringLength(20)]
        public string Publish { get; set; }
        [Required]
        public int PubyearLower { get; set; }
        [Required]
        public int PubyearUpper { get; set; }
        [Required]
        [StringLength(20)]
        public string Author { get; set; }
        [Required]
        public double PriceLower { get; set; }
        [Required]
        public double PriceUpper { get; set; }
    }
    
    
    public class CardReqDto
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        [Required]
        [StringLength(20)]
        public string Company { get; set; }
        [Required]
        [StringLength(20)]
        public string Class { get; set; }
    }
    
    
}