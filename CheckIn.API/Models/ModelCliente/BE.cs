﻿ 

namespace CheckIn.API.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BE")]
    public partial class BE
    {
        public int id { get; set; }
        public string Descripcion { get; set; }
        public string StackTrace { get; set; }
        public DateTime Fecha { get; set; }

    }
}