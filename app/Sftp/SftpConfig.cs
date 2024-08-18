using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace positive_pay_app.app.Sftp
{
    public class SftpConfig
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Directory { get; set; }
    }
}
