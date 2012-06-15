using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Models
{
    public class RequestMessage
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }
        public String Token { get; set; }
        public String DateList { get; set; }
    }
}