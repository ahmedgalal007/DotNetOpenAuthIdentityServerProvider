using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.MVC.Models
{
    
    public class OrderFrameViewModel
    {
        public string Address { get; private set; } = string.Empty;
        public OrderFrameViewModel(string address)
        {
            Address = address;
        }
    }
}
