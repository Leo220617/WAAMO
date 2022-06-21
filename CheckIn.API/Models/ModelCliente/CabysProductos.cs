namespace CheckIn.API.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CabysProductos
    {
        public int id { get; set; }

        [StringLength(15)]
        public string Cabys { get; set; }

        [StringLength(5000)]
        public string Descripcion { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        public string PalabrasClaves { get; set; }
    }
}
