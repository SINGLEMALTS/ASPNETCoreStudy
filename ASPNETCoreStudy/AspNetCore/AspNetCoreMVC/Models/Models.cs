using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMVC.Models
{
    //public class TestModels
    //{
    //    public int id { get; set; }
    //    public string Name { get; set; }
    //}

    public class TestModels
    {
        // 검증을 위한 Annotation 추가
        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
